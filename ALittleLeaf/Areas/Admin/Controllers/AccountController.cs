using ALittleLeaf.Controllers;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AccountController(AlittleLeafDecorContext context)
        {
            _context = context;
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
                var hashPass = new HashPasswordController();

                var user = _context.Users
                    .FirstOrDefault(u => u.UserEmail == model.UserEmail && u.UserRole == "admin");

                if (user != null)
                {
                    bool isPasswordCorrect = hashPass.VerifyPassword(user.UserPassword, model.UserPassword);

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
