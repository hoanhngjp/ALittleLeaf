using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace ALittleLeaf.Controllers
{
    public class AdminUserInfoController : Controller
    {
        private readonly AlittleLeafDecorContext _context;
        public AdminUserInfoController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(long userId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var user = _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    UserEmail = u.UserEmail,
                    UserFullname = u.UserFullname,
                    UserPassword = u.UserPassword,
                    UserBirthday = u.UserBirthday,
                    UserIsActive = u.UserIsActive,
                    UserRole = u.UserRole,
                    UserSex = u.UserSex,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,

                })
                .SingleOrDefault();


            if (user == null)
            {
                return NotFound();
            }

            // Tạo ViewModel và điền dữ liệu
            var viewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                UserFullname = user.UserFullname,
                UserSex = user.UserSex,
                UserBirthday = user.UserBirthday,
                UserIsActive = user.UserIsActive,
                UserRole = user.UserRole 
            };

            return View(viewModel);
        }
    }
}
