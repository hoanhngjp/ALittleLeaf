using ALittleLeaf.Filters;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.Services.Order;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AccountController : SiteBaseController
    {
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        public AccountController(IAuthService authService, IOrderService orderService)
        {
            _authService = authService;
            _orderService = orderService;
        }

        [CheckLogin]
        public async Task<IActionResult> Index()
        {
            // Đây là trang Lịch sử đơn hàng
            var userId = long.Parse(HttpContext.Session.GetString("UserId"));
            var orders = await _orderService.GetOrderHistoryAsync(userId);

            return View(orders); 
            // Sẽ tìm View tại: /Views/Account/Index.cshtml
        }

        [CheckLogin]
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id) // 'id' là BillId
        {
            var userId = long.Parse(HttpContext.Session.GetString("UserId"));

            // 1. Lấy thông tin Bill chính (có kiểm tra bảo mật)
            var bill = await _orderService.GetBillByIdAsync(id, userId);

            if (bill == null)
            {
                // Người này đang cố xem đơn hàng của người khác, hoặc đơn không tồn tại
                return NotFound();
            }

            // 2. Lấy chi tiết các sản phẩm
            var billDetails = await _orderService.GetBillDetailsAsync(id);

            // 3. Gửi cả 2 ra View
            ViewBag.Bill = bill; // Gửi thông tin chính (địa chỉ, tổng tiền...)
            return View(billDetails); // Gửi model là List<OrderDetailViewModel>
            // Sẽ tìm View tại: /Views/Account/OrderDetails.cshtml
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserLoggedViewModel model, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Account");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
            // Sẽ tìm View tại: /Views/Account/Register.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Account");
            }

            ModelState.AddModelError("UserEmail", result.ErrorMessage);
            return View(model);
        }

        // ---------- LOGOUT ----------
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home"); 
        }
    }
}
