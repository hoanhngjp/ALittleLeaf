using ALittleLeaf.Filters;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.Services.Order;
using ALittleLeaf.Utils;
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
            var userId = User.GetUserId();

            if (userId == 0) return RedirectToAction("Logout");

            var orders = await _orderService.GetOrderHistoryAsync(userId);

            return View(orders); 
            // Sẽ tìm View tại: /Views/Account/Index.cshtml
        }

        [CheckLogin]
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id) // 'id' là BillId
        {
            var userId = User.GetUserId();

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
                // LƯU COOKIE
                Response.Cookies.Append("X-Access-Token", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                TempData["SuccessMessage"] = "Đăng nhập thành công";

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)) return Redirect(ReturnUrl);
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = result.ErrorMessage;

            ViewBag.ReturnUrl = ReturnUrl;
            
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
            // 1. Gọi Service để Revoke Token trong DB (Xử lý LOGOUT-001, 002, 004)
            // Dù Service trả về true hay false, ta vẫn phải xóa Cookie để đảm bảo người dùng thoát hẳn (Xử lý LOGOUT-005)
            await _authService.LogoutAsync();

            // 2. Xóa toàn bộ dữ liệu phiên làm việc (Cookie & Session)
            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token"); // Xóa cả Refresh Token
            HttpContext.Session.Clear();

            // 3. Hiển thị thông báo (LOGOUT-001 yêu cầu: "Bạn đã đăng xuất thành công.")
            // Dùng TempData để truyền thông báo sang trang Login
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";

            // 4. Chuyển hướng (LOGOUT-001 yêu cầu: "Chuyển hướng về trang Đăng nhập")
            // Lưu ý: Test case yêu cầu về trang Đăng nhập, KHÔNG PHẢI trang chủ.
            return RedirectToAction("Login", "Account");
        }
    }
}
