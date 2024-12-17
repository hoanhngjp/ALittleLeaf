using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminLoginController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        public IActionResult LoginAdmin(UserLoggedViewModel model, string? ReturnUrl) {
            if (ModelState.IsValid)
            {
                var hashPass = new HashPasswordController();

                var user = _context.Users
                    .FirstOrDefault(u => u.UserEmail == model.UserEmail && u.UserRole == "admin");

                if (user != null)
                {
                    // Kiểm tra mật khẩu đã nhập với mật khẩu đã hash trong DB
                    bool isPasswordCorrect = hashPass.VerifyPassword(user.UserPassword, model.UserPassword);

                    if (isPasswordCorrect)
                    {
                        if (user.UserIsActive)
                        {
                            HttpContext.Session.SetString("AdminEmail", user.UserEmail);
                            HttpContext.Session.SetString("AdminFullname", user.UserFullname);
                            HttpContext.Session.SetString("AdminId", user.UserId.ToString());

                            // Kiểm tra nếu ReturnUrl hợp lệ
                            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl); // Chuyển hướng đến ReturnUrl
                            }
                            else
                            {
                                return RedirectToAction("Index", "Admin"); // Nếu không có ReturnUrl, chuyển về trang chủ
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa.";
                        }
                    }    
                }
                else
                {
                    ViewBag.ErrorMessage = "Thông tin đăng nhập không hợp lệ.";
                }
            }
            return View("Index");
        }
        public IActionResult LogoutAdmin()
        {
            // Clear toàn bộ Session
            HttpContext.Session.Clear();

            // Đăng xuất khỏi Cookie Authentication
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng người dùng đến trang đăng nhập
            return RedirectToAction("Index", "AdminLogin");
        }
    }
}
