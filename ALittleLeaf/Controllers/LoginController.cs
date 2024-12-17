using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ALittleLeaf.Controllers
{
    public class LoginController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public LoginController(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            ViewData["Cart"] = cart;

            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoggedViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var hashPass = new HashPasswordController();

                var user = _context.Users
                    .FirstOrDefault(u => u.UserEmail == model.UserEmail);

                if (user != null)
                {
                    // Kiểm tra mật khẩu đã nhập với mật khẩu đã hash trong DB
                    bool isPasswordCorrect = hashPass.VerifyPassword(user.UserPassword, model.UserPassword);

                    if (isPasswordCorrect)
                    {
                        if (user.UserIsActive)
                        {
                            HttpContext.Session.SetString("UserEmail", user.UserEmail);
                            HttpContext.Session.SetString("UserFullname", user.UserFullname);
                            HttpContext.Session.SetString("UserId", user.UserId.ToString());

                            // Kiểm tra nếu ReturnUrl hợp lệ
                            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl); // Chuyển hướng đến ReturnUrl
                            }
                            else
                            {
                                return RedirectToAction("Index", "Account"); // Nếu không có ReturnUrl, chuyển về trang chủ
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa.";
                        }
                    }  
                    else
                    {
                        ViewBag.ErrorMessage = "Thông tin đăng nhập không hợp lệ.";
                    }    
                }
                else
                {
                    ViewBag.ErrorMessage = "Thông tin đăng nhập không hợp lệ.";
                }
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Cart")))
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

                ViewData["Cart"] = cart;
            }

            return View("Index");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear toàn bộ Session
            HttpContext.Session.Clear();

            // Đăng xuất khỏi Cookie Authentication
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Account");
        }

    }
}
