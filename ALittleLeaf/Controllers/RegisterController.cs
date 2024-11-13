using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public RegisterController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email có tồn tại trong cơ sở dữ liệu không
                if (_context.Users.Any(u => u.UserEmail == model.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email đã tồn tại.");
                    return View(model);
                }

                // Tạo một người dùng mới
                var user = new User
                {
                    UserEmail = model.UserEmail,
                    UserPassword = model.UserPassword, // Đảm bảo mã hóa mật khẩu sau này
                    UserFullname = model.UserFullname,
                    UserSex = model.UserSex,
                    UserBirthday = model.UserBirthday,
                    UserIsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserRole = "customer" // Gán role mặc định là User
                };

                // Thêm người dùng vào cơ sở dữ liệu
                _context.Users.Add(user);
                _context.SaveChanges();

                // Chuyển hướng sau khi đăng ký thành công
                return RedirectToAction("Index", "Home");
            }

            return View(model); // Nếu không hợp lệ, trả về view với lỗi
        }

        // Trang thành công sau khi đăng ký
        public IActionResult Success()
        {
            return View();
        }
    }
}
