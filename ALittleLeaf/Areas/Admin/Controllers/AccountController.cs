using ALittleLeaf.Controllers;
using ALittleLeaf.Repository;
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
        private readonly AlittleLeafDecorContext _context;
        private readonly IPasswordHasher<string> _passwordHasher;

        public AccountController(AlittleLeafDecorContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<string>();
        }

        // GET: /Admin/Account/Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        // POST: /Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoggedViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                // Tìm user có role admin
                var user = _context.Users
                    .FirstOrDefault(u => u.UserEmail == model.UserEmail && u.UserRole == "admin");

                if (user != null)
                {
                    var result = _passwordHasher.VerifyHashedPassword(null, user.UserPassword, model.UserPassword);
                    bool isPasswordCorrect = result == PasswordVerificationResult.Success;

                    if (isPasswordCorrect)
                    {
                        if (user.UserIsActive)
                        {
                            HttpContext.Session.SetString("AdminEmail", user.UserEmail);
                            HttpContext.Session.SetString("AdminFullname", user.UserFullname);
                            HttpContext.Session.SetString("AdminId", user.UserId.ToString());

                            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Dashboard");
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
            return View("Login", model);
        }

        // GET: /Admin/Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }
    }
}
