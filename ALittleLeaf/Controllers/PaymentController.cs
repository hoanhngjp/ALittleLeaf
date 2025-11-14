using ALittleLeaf.Filters;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.VNPay;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    [CheckLogin]
    public class PaymentController : SiteBaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly AlittleLeafDecorContext _context;

        public PaymentController(IVnPayService vnPayService, AlittleLeafDecorContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        // Kế thừa logic tạo Bill từ CheckOutController
        // NHƯNG CHƯA TRỪ KHO
        private Bill SavePendingBill()
        {
            var userId = long.Parse(HttpContext.Session.GetString("UserId"));
            int adrsId;

            // Xử lý địa chỉ (giống hệt code của bạn)
            if (HttpContext.Session.GetString("BillingAdrsId") == "new")
            {
                string AdrsFullname = HttpContext.Session.GetString("BillingFullName");
                string AdrsAddress = HttpContext.Session.GetString("BillingAddress");
                string AdrsPhone = HttpContext.Session.GetString("BillingPhone");

                var newAddress = new AddressList
                {
                    AdrsFullname = AdrsFullname,
                    AdrsAddress = AdrsAddress,
                    AdrsPhone = AdrsPhone,
                    AdrsIsDefault = false,
                    IdUser = userId
                };
                _context.AddressLists.Add(newAddress);
                _context.SaveChanges(); // Lưu để lấy ID
                adrsId = newAddress.AdrsId;
            }
            else
            {
                adrsId = int.Parse(HttpContext.Session.GetString("BillingAdrsId"));
            }

            // Tạo Bill
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart");
            int totalAmount = cart.Sum(c => c.Quantity * c.ProductPrice);
            string Note = HttpContext.Session.GetString("BillNote");

            var bill = new Bill
            {
                IdUser = userId,
                IdAdrs = adrsId,
                DateCreated = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = totalAmount,
                PaymentMethod = "VNPAY",
                PaymentStatus = "pending_vnpay", // Trạng thái chờ mới
                IsConfirmed = false, // Chưa xác nhận
                ShippingStatus = "not_fulfilled",
                Note = Note,
                UpdatedAt = DateTime.Now
            };
            _context.Bills.Add(bill);
            _context.SaveChanges(); // Lưu Bill để lấy BillId

            // Lưu BillDetails
            foreach (var item in cart)
            {
                var billDetail = new BillDetail
                {
                    IdBill = bill.BillId,
                    IdProduct = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.ProductPrice,
                    TotalPrice = item.Quantity * item.ProductPrice,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.BillDetails.Add(billDetail);
            }
            _context.SaveChanges(); // Lưu BillDetails

            return bill;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrl()
        {
            // 1. Lưu đơn hàng (trạng thái chờ) vào DB
            var bill = SavePendingBill();

            // 2. Tạo model thanh toán
            var model = new PaymentInformationModel
            {
                Amount = bill.TotalAmount,
                Name = "Khach hang " + bill.IdUser,
                OrderDescription = $"Thanh toan don hang {bill.BillId}",
                OrderType = "other"
            };

            // 3. Tạo URL VNPAY
            // Chúng ta dùng BillId làm TxnRef (mã giao dịch)
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, bill.BillId.ToString());

            // 4. Chuyển hướng người dùng đến VNPAY
            return Redirect(url);
        }

        // Action mà VNPAY gọi về
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || !response.Success || response.VnPayResponseCode != "00")
            {
                // Lấy BillId từ response
                int.TryParse(response?.OrderId, out int billId);

                // Xóa Bill và BillDetails đã tạo
                if (billId > 0)
                {
                    var bill = _context.Bills
                        .Include(b => b.BillDetails)
                        .FirstOrDefault(b => b.BillId == billId);

                    if (bill != null && bill.PaymentStatus == "pending_vnpay")
                    {
                        _context.BillDetails.RemoveRange(bill.BillDetails);
                        _context.Bills.Remove(bill);
                        _context.SaveChanges();
                    }
                }

                // Thanh toán thất bại
                return View("PaymentFail");
            }

            // --- THANH TOÁN THÀNH CÔNG ---

            // 1. Cập nhật Bill
            int billIdSuccess = int.Parse(response.OrderId);

            // Bắt đầu một Transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var successBill = await _context.Bills
                        .Include(b => b.BillDetails)
                        .FirstOrDefaultAsync(b => b.BillId == billIdSuccess);

                    // !! QUAN TRỌNG: Kiểm tra xem nó đã được IPN xử lý chưa
                    if (successBill != null && successBill.PaymentStatus == "pending_vnpay")
                    {
                        successBill.PaymentStatus = "paid"; // Đã thanh toán
                        successBill.IsConfirmed = true; // Xác nhận đơn

                        // Trừ kho
                        foreach (var item in successBill.BillDetails)
                        {
                            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.IdProduct);
                            if (product != null)
                            {
                                if (product.QuantityInStock < item.Quantity)
                                {
                                    // Nếu lỡ hết hàng trong lúc thanh toán
                                    throw new Exception($"San pham {product.ProductName} khong du hang.");
                                }
                                product.QuantityInStock -= item.Quantity;
                            }
                        }

                        // Mọi thứ OK, lưu tất cả thay đổi
                        await _context.SaveChangesAsync();

                        // Commit Transaction
                        await transaction.CommitAsync();

                        // Xóa Session (chỉ làm khi thành công)
                        HttpContext.Session.Remove("BillingFullName");
                        // ... (xóa các session khác) ...
                        HttpContext.Session.Remove("Cart");
                    }
                    // Nếu PaymentStatus != "pending_vnpay" -> nghĩa là IPN đã chạy trước, 
                    // chúng ta không cần làm gì, chỉ cần hiển thị trang thành công.
                }
                catch (Exception ex)
                {
                    // Nếu có bất kỳ lỗi nào (ví dụ: hết hàng), hủy bỏ mọi thay đổi
                    await transaction.RollbackAsync();
                    // Log lỗi ex
                    return View("PaymentFail"); // Trả về trang thất bại
                }
            }
            // 4. Trả về trang thành công
            ViewBag.BillId = billIdSuccess;
            return View("PaymentSuccess");
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> VnpayIpnCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || !response.Success || response.VnPayResponseCode != "00")
            {
                // IPN thất bại, log lại
                // Console.WriteLine("IPN Failed: " + response.VnPayResponseCode);

                // Trả về lỗi cho VNPAY. VNPAY sẽ thử gọi lại.
                return Json(new { RspCode = "01", Message = "Fail" });
            }

            // --- IPN THÀNH CÔNG ---
            int.TryParse(response.OrderId, out int billId);
            if (billId <= 0)
            {
                return Json(new { RspCode = "01", Message = "Fail (Invalid OrderId)" });
            }

            // Bắt đầu một Transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var bill = await _context.Bills
                        .Include(b => b.BillDetails)
                        .FirstOrDefaultAsync(b => b.BillId == billId);

                    // !! QUAN TRỌNG: Chỉ xử lý nếu đơn hàng đang "chờ"
                    // Nếu trạng thái là "paid" (do PaymentCallbackVnpay đã chạy trước),
                    // chúng ta sẽ bỏ qua và trả về "Success" (để VNPAY không gọi lại nữa).
                    if (bill != null && bill.PaymentStatus == "pending_vnpay")
                    {
                        bill.PaymentStatus = "paid"; // Đã thanh toán
                        bill.IsConfirmed = true;     // Xác nhận đơn

                        // Trừ kho
                        foreach (var item in bill.BillDetails)
                        {
                            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.IdProduct);
                            if (product != null)
                            {
                                if (product.QuantityInStock < item.Quantity)
                                {
                                    // Nếu lỡ hết hàng, ghi log và trả về lỗi
                                    // VNPAY sẽ thử gọi lại IPN sau.
                                    // Console.WriteLine($"IPN Error: San pham {product.ProductName} khong du hang.");
                                    await transaction.RollbackAsync();
                                    return Json(new { RspCode = "02", Message = "Fail (Out of Stock)" });
                                }
                                product.QuantityInStock -= item.Quantity;
                            }
                        }

                        // Mọi thứ OK, lưu tất cả thay đổi
                        await _context.SaveChangesAsync();

                        // Commit Transaction
                        await transaction.CommitAsync();

                        // Console.WriteLine("IPN Success for BillId: " + billId);
                    }
                    // else: Đơn hàng đã được xử lý (PaymentCallbackVnpay nhanh hơn),
                    // không cần làm gì, chỉ cần trả về Success.
                }
                catch (Exception ex)
                {
                    // Nếu có bất kỳ lỗi CSDL nào
                    await transaction.RollbackAsync();
                    // Console.WriteLine("IPN Exception: " + ex.Message);
                    return Json(new { RspCode = "99", Message = "Fail (Exception)" });
                }
            }

            // Luôn trả về "00" cho VNPAY nếu mọi thứ (kể cả việc "bỏ qua") đều ổn
            return Json(new { RspCode = "00", Message = "Success" });
        }
    }
}
