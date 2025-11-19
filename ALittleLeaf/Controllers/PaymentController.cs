using ALittleLeaf.Filters;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Order;
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
        private readonly IOrderService _orderService; // <-- SỬA
        private readonly AlittleLeafDecorContext _context;

        public PaymentController(IVnPayService vnPayService, IOrderService orderService, AlittleLeafDecorContext context) // <-- SỬA
        {
            _vnPayService = vnPayService;
            _orderService = orderService; // <-- SỬA
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl()
        {
            // 1. Lưu đơn hàng (trạng thái chờ) vào DB
            var bill = await _orderService.CreateOrderFromSessionAsync("VNPAY", "pending_vnpay");

            // 2. Tạo model thanh toán
            var model = new PaymentInformationModel { /* ... */ };
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, bill.BillId.ToString());

            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || !response.Success || response.VnPayResponseCode != "00")
            {
                return View("PaymentFail");
            }

            // --- THANH TOÁN THÀNH CÔNG ---
            int billIdSuccess = int.Parse(response.OrderId);

            try
            {
                var bill = await _context.Bills.FindAsync(billIdSuccess);

                // Kiểm tra xem đơn hàng có "pending" không (tránh IPN xử lý 2 lần)
                if (bill != null && bill.PaymentStatus == "pending_vnpay")
                {
                    // Cập nhật trạng thái
                    bill.PaymentStatus = "paid";
                    bill.IsConfirmed = true;
                    await _context.SaveChangesAsync();

                    // Xử lý đơn hàng (Trừ kho, Xóa cart)
                    await _orderService.FulfillOrderAsync(billIdSuccess);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi (ex.Message)
                return View("PaymentFail"); // Trả về trang thất bại nếu FulfillOrderAsync lỗi
            }

            ViewBag.BillId = billIdSuccess;
            return View("PaymentSuccess");
        }

        [HttpGet]
        public async Task<IActionResult> VnpayIpnCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            // ... (check response) ...
            int.TryParse(response.OrderId, out int billId);

            try
            {
                var bill = await _context.Bills.FindAsync(billId);
                if (bill != null && bill.PaymentStatus == "pending_vnpay")
                {
                    // Cập nhật trạng thái
                    bill.PaymentStatus = "paid";
                    bill.IsConfirmed = true;
                    await _context.SaveChangesAsync();

                    // Xử lý đơn hàng (Trừ kho, Xóa cart)
                    await _orderService.FulfillOrderAsync(billId);
                }
            }
            catch (Exception ex)
            {
                // ... (Xử lý lỗi) ...
                return Json(new { RspCode = "99", Message = "Fail (Exception)" });
            }

            return Json(new { RspCode = "00", Message = "Success" });
        }

        [CheckLogin]
        [HttpGet]
        public async Task<IActionResult> RetryPayment(int billId)
        {
            var userId = long.Parse(HttpContext.Session.GetString("UserId"));

            // 1. Lấy đơn hàng và kiểm tra bảo mật (đơn hàng phải là của user này)
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillId == billId && b.IdUser == userId);

            // 2. Kiểm tra xem đơn hàng có hợp lệ để thanh toán lại không
            if (bill == null || bill.PaymentStatus != "pending_vnpay")
            {
                // Nếu đã thanh toán, hoặc là đơn COD, thì không cho thanh toán lại
                TempData["Error"] = "Đơn hàng này không thể thanh toán lại.";
                return RedirectToAction("Index", "Account");
            }

            // 3. Tạo model thanh toán (giống CreatePaymentUrl)
            var model = new PaymentInformationModel
            {
                Amount = bill.TotalAmount,
                Name = "Khach hang " + bill.IdUser,
                OrderDescription = $"Thanh toan lai don hang {bill.BillId}",
                OrderType = "other"
            };

            // 4. Tạo URL VNPAY mới
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, bill.BillId.ToString());

            // 5. Chuyển hướng người dùng
            return Redirect(url);
        }
    }
}
