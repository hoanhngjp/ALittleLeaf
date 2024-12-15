using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.WebSockets;


namespace ALittleLeaf.Controllers
{

    public class CheckOutController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public CheckOutController(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            ViewData["Cart"] = cart;

            return View();
        }

        public IActionResult SaveCheckOutMethod(string checkoutMethod)
        {
            if (HttpContext.Session.GetString("BillingAdrsId") == "new")
            {
                var userId = long.Parse(HttpContext.Session.GetString("UserId"));

                string AdrsFullname = HttpContext.Session.GetString("BillingFullName");
                string AdrsAddress = HttpContext.Session.GetString("BillingAddress");
                string AdrsPhone = HttpContext.Session.GetString("BillingPhone");
                int AdrsIsDefault = 0;

                var newAddress = new AddressList
                {
                    AdrsFullname = AdrsFullname,
                    AdrsAddress = AdrsAddress,
                    AdrsPhone = AdrsPhone,
                    AdrsIsDefault = AdrsIsDefault == 1, // Chuyển đổi thành kiểu bool
                    IdUser = userId // Giả sử bạn có cách lấy ID người dùng hiện tại
                };

                // Nếu địa chỉ mới được đặt làm mặc định, cập nhật các địa chỉ khác
                if (newAddress.AdrsIsDefault)
                {
                    var otherAddresses = _context.AddressLists.Where(a => a.IdUser == userId);

                    foreach (var address in otherAddresses)
                    {
                        address.AdrsIsDefault = false;
                    }
                }

                // Lưu địa chỉ mới vào cơ sở dữ liệu
                _context.AddressLists.Add(newAddress);
                _context.SaveChanges();

                // Truy vấn lại để lấy ID của bản ghi vừa thêm
                var addedAddress = _context.AddressLists
                    .Where(a => a.AdrsFullname == AdrsFullname
                                && a.AdrsAddress == AdrsAddress
                                && a.AdrsPhone == AdrsPhone
                                && a.IdUser == userId)
                    .OrderByDescending(a => a.AdrsId)// Sắp xếp giảm dần để lấy bản ghi mới nhất
                    .FirstOrDefault();

                var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart");

                int adrsId = addedAddress.AdrsId;
                DateOnly dateCreated = DateOnly.FromDateTime(DateTime.Now);
                int totalAmount = cart.Sum(c => c.Quantity * c.ProductPrice);
                string PaymentMethod = checkoutMethod;
                string PaymentStatus = "pending";
                int IsConfirmed = 0;
                string ShippingStatus = "not_fulfilled";
                string Note = HttpContext.Session.GetString("BillNote");
                DateTime UpdatedAt = DateTime.Now;

                SaveBill(cart, userId, adrsId, dateCreated, totalAmount, PaymentMethod, PaymentStatus, IsConfirmed, ShippingStatus, Note, UpdatedAt);
            }
            else
            {
                var userId = long.Parse(HttpContext.Session.GetString("UserId"));
                var adrsId = int.Parse(HttpContext.Session.GetString("BillingAdrsId"));
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart");
                DateOnly dateCreated = DateOnly.FromDateTime(DateTime.Now);
                int totalAmount = cart.Sum(c => c.Quantity * c.ProductPrice);
                string PaymentMethod = checkoutMethod;
                string PaymentStatus = "pending";
                int IsConfirmed = 0;
                string ShippingStatus = "not_fulfilled";
                string Note = HttpContext.Session.GetString("BillNote");
                DateTime UpdatedAt = DateTime.Now;

                SaveBill(cart, userId, adrsId, dateCreated, totalAmount, PaymentMethod, PaymentStatus, IsConfirmed, ShippingStatus, Note, UpdatedAt);

            }   
            return RedirectToAction("Index", "Account");
        }
        public void SaveBill(List<CartItemViewModel> cart, long userId, int adrsId, DateOnly dateCreated, int totalAmount, string PaymentMethod, string PaymentStatus, int IsConfirmed, string ShippingStatus,
                            string Note, DateTime UpdatedAt)
        {
            var bill = new Bill
            {
                IdUser = userId,
                IdAdrs = adrsId,
                DateCreated = dateCreated,
                TotalAmount = totalAmount,
                PaymentMethod = PaymentMethod,
                PaymentStatus = PaymentStatus,
                IsConfirmed = IsConfirmed == 0,
                ShippingStatus = ShippingStatus,
                Note = Note,
                UpdatedAt = UpdatedAt
            };

            _context.Bills.Add(bill);
            _context.SaveChanges();

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

                // Trừ số lượng sản phẩm trong kho
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.QuantityInStock -= item.Quantity; // Giảm số lượng
                    if (product.QuantityInStock < 0)
                    {
                        throw new InvalidOperationException($"Sản phẩm {product.ProductName} không đủ số lượng trong kho!");
                    }
                }

                _context.SaveChanges();
            }

            HttpContext.Session.Remove("BillingFullName");
            HttpContext.Session.Remove("BillingAddress");
            HttpContext.Session.Remove("BillingPhone");
            HttpContext.Session.Remove("BillNote");
            HttpContext.Session.Remove("BillingAdrsId");
            HttpContext.Session.Remove("Cart");

        }

    }
}
