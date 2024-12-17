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
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            ViewData["Cart"] = cart;

            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email có tồn tại trong cơ sở dữ liệu không
                if (_context.Users.Any(u => u.UserEmail == model.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email đã tồn tại.");
                    if(string.IsNullOrEmpty(HttpContext.Session.GetString("Cart"))){
                        var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

                        ViewData["Cart"] = cart;

                        return View("Index");
                    }
                    return View("Index");
                }

                var hashPass = new HashPasswordController();

                string hashedPass = hashPass.HashPassword(model.UserPassword);

                // Tạo một người dùng mới
                var user = new User
                {
                    UserEmail = model.UserEmail,
                    UserPassword = hashedPass, 
                    UserFullname = model.UserFullname,
                    UserSex = model.UserSex,
                    UserBirthday = model.UserBirthday,
                    UserIsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserRole = model.UserRole
                };
                // Thêm người dùng vào cơ sở dữ liệu
                _context.Users.Add(user);
                _context.SaveChanges();

                // Lưu thông tin người dùng vào Session
                HttpContext.Session.SetString("UserEmail", user.UserEmail);
                HttpContext.Session.SetString("UserFullname", user.UserFullname);
                HttpContext.Session.SetString("UserId", user.UserId.ToString());

                // Lưu địa chỉ mặc định vào AddressList
                var address = new AddressList
                {
                    IdUser = user.UserId,
                    AdrsFullname = user.UserFullname,
                    AdrsAddress = model.Address, // Địa chỉ từ model đăng ký
                    AdrsPhone = "N/A",
                    AdrsIsDefault = true, // Đặt làm địa chỉ mặc định
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.AddressLists.Add(address);
                _context.SaveChanges();

                // Chuyển hướng sau khi đăng ký thành công
                return RedirectToAction("Index", "Account");
            }
            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
            }
            return View("Index"); // Nếu không hợp lệ, trả về view với lỗi
        }
    }
}
