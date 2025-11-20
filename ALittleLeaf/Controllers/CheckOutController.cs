using ALittleLeaf.Filters;
using ALittleLeaf.Repository;
using ALittleLeaf.Services;
using ALittleLeaf.Services.Order;
using ALittleLeaf.Utils;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ALittleLeaf.Controllers
{
    [CheckLogin]
    public class CheckOutController : SiteBaseController
    {
        private readonly IOrderService _orderService;
        private readonly AlittleLeafDecorContext _context; // Tạm giữ

        // SỬA: Inject IOrderService
        public CheckOutController(IOrderService orderService, AlittleLeafDecorContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        // --- BƯỚC 1: NHẬP THÔNG TIN GIAO HÀNG ---
        // (Đây là Action Index() cũ của OrderInfoController)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = ViewData["Cart"] as List<CartItemViewModel>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Collections");
            }

            var userId = User.GetUserId();

            if (userId == 0) return RedirectToAction("Logout");

            // Gọi Service để lấy địa chỉ
            ViewData["Addresses"] = await _orderService.GetUserAddressesAsync(userId);

            return View(); // Sẽ dùng View: /Views/CheckOut/Index.cshtml
        }

        // (Action GetAddressInfo() cũ của OrderInfoController)
        [HttpPost]
        public async Task<IActionResult> GetAddressInfo(int addressId)
        {
            if (addressId <= 0) return Json(new { error = "Invalid address ID" });

            // Gọi Service
            var address = await _orderService.GetAddressDetailsAsync(addressId);

            if (address == null) return Json(new { error = "Address not found" });

            return Json(new
            {
                adrs_id = address.AdrsId,
                adrs_fullname = address.AdrsFullname,
                adrs_phone = address.AdrsPhone,
                adrs_address = address.AdrsAddress
            });
        }

        // (Action SaveAddressInfo() cũ của OrderInfoController)
        [HttpPost]
        public IActionResult SaveShippingInfo(string bill_addressId, string billing_address_full_name, string billing_address_phone, string billing_address_address)
        {
            // Logic lưu vào Session này là OK
            if (bill_addressId == null)
            {
                HttpContext.Session.SetString("BillingAdrsId", "new");
            }
            else
            {
                HttpContext.Session.SetString("BillingAdrsId", bill_addressId);
            }
            HttpContext.Session.SetString("BillingFullName", billing_address_full_name);
            HttpContext.Session.SetString("BillingPhone", billing_address_phone);
            HttpContext.Session.SetString("BillingAddress", billing_address_address);

            // SỬA: Chuyển hướng đến Bước 2 (Payment)
            return RedirectToAction("Payment", "CheckOut");
        }

        // --- BƯỚC 2: CHỌN PHƯƠNG THỨC THANH TOÁN ---
        // (Đây là Action Index() cũ của CheckOutController)
        [HttpGet]
        public IActionResult Payment()
        {
            var cart = ViewData["Cart"] as List<CartItemViewModel>;
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(); // Sẽ dùng View: /Views/CheckOut/Payment.cshtml
        }

        // (Action SaveCheckOutMethod() cũ, giờ chỉ xử lý COD)
        [HttpPost]
        public async Task<IActionResult> PlaceCodOrder(string checkoutMethod)
        {
            try
            {
                // 1. Tạo đơn hàng (pending)
                // (Giả sử COD có 'checkoutMethod' là "cod", Chuyển khoản là "online")
                var bill = await _orderService.CreateOrderFromSessionAsync(checkoutMethod, "pending");

                // 2. Xử lý đơn hàng (Trừ kho, Xóa cart) ngay lập tức
                await _orderService.FulfillOrderAsync(bill.BillId);

                // 3. Chuyển hướng
                return RedirectToAction("Index", "Account"); // Hoặc trang "Cảm ơn"
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}