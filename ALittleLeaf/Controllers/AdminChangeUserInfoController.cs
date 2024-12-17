using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ALittleLeaf.Controllers
{
    public class AdminChangeUserInfoController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminChangeUserInfoController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            int pageSize = 10;

            var users = _context.Users.AsQueryable();

            var result = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    UserEmail = u.UserEmail,
                    UserFullname = u.UserFullname,
                    UserBirthday = u.UserBirthday,
                    UserIsActive = u.UserIsActive,
                    UserRole = u.UserRole,
                    UserSex = u.UserSex,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    
                }).ToList();

            int totalItems = users.Count();

            var pagination = new Paginate(totalItems, page, pageSize);

            ViewBag.Pagination = pagination;


            return View(result);
        }
        [HttpPost]
        public IActionResult ChangeUserInfo(long UserId, string UserEmail, string? UserNewPassword, string UserFullname, bool UserSex, DateOnly UserBirthday, bool UserIsActive, string UserRole)
        {
            // Tìm người dùng trong cơ sở dữ liệu
            var user = _context.Users.FirstOrDefault(u => u.UserId == UserId);

            if (user == null)
            {
                // Nếu không tìm thấy người dùng, chuyển hướng với thông báo lỗi
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction("Index", "AdminChangeUserInfo");
            }

            // Cập nhật thông tin người dùng
            user.UserEmail = UserEmail;
            user.UserFullname = UserFullname;
            user.UserSex = UserSex;
            user.UserBirthday = UserBirthday;
            user.UserIsActive = UserIsActive;
            user.UserRole = UserRole;

            // Hash mật khẩu mới nếu có thay đổi
            if (!string.IsNullOrEmpty(UserNewPassword))
            {
                var hashPass = new HashPasswordController(); // Giả sử bạn có sẵn lớp này
                user.UserPassword = hashPass.HashPassword(UserNewPassword);
            }

            try
            {
                user.UpdatedAt = DateTime.Now;
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Lưu thông báo thành công
                TempData["SuccessMessage"] = "Cập nhật thông tin người dùng thành công.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật thông tin người dùng: " + ex.Message;
            }

            // Chuyển hướng về trang quản lý
            return RedirectToAction("Index", "AdminChangeUserInfo");
        }

        public IActionResult SearchUser(string searchType, string searchKey, int page = 1, int pageSize = 10)
        {
            var usersQuery = _context.Users.AsQueryable();

            // Áp dụng logic tìm kiếm
            if (!string.IsNullOrEmpty(searchKey))
            {
                switch (searchType)
                {
                    case "findByID":
                        usersQuery = usersQuery.Where(u => u.UserId.ToString().Contains(searchKey));
                        break;
                    case "findByEmail":
                        usersQuery = usersQuery.Where(u => u.UserEmail.Contains(searchKey));
                        break;
                    case "findByName":
                        usersQuery = usersQuery.Where(u => u.UserFullname.Contains(searchKey));
                        break;
                }
            }

            // Tổng số bản ghi
            int totalItems = usersQuery.Count();

            // Lấy danh sách các user thuộc trang hiện tại
            var users = usersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Nếu không có kết quả, tạo ViewBag chứa thông báo
            if (!users.Any())
            {
                ViewBag.Message = "Không tìm thấy người dùng nào phù hợp với từ khóa tìm kiếm.";
            }

            // Tạo ViewModel để truyền dữ liệu
            var model = new UserSearchViewModel
            {
                Users = users,
                Pagination = new Paginate(totalItems, page, pageSize)
            };

            return PartialView("_SearchUserResult", model);
        }
    }
}
