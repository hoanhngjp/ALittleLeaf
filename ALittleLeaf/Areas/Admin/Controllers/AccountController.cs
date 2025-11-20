using ALittleLeaf.Controllers;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoggedViewModel model, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.LoginAsync(model);

            if (result.Succeeded)
            {
                // Kiểm tra quyền Admin
                if (result.User.UserRole != "admin")
                {
                    ViewBag.ErrorMessage = "Bạn không có quyền truy cập Admin.";
                    return View(model);
                }

                // LƯU COOKIE
                Response.Cookies.Append("X-Access-Token", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                // Lưu Refresh Token (nếu cần dùng để renew sau này)
                Response.Cookies.Append("X-Refresh-Token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true, 
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(30)
                });

                // Vẫn lưu Session cho các logic cũ chưa kịp chuyển (nếu có)
                HttpContext.Session.SetString("AdminEmail", result.User.UserEmail);
                HttpContext.Session.SetString("AdminFullname", result.User.UserFullname);

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)) return Redirect(ReturnUrl);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ErrorMessage = result.ErrorMessage;
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa Cookie
            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token");

            // Xóa Session
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}
