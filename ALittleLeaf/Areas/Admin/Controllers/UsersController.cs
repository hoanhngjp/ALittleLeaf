using ALittleLeaf.Areas.Admin.ViewModels;
using ALittleLeaf.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        private readonly AlittleLeafDecorContext _context;
        public UsersController(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        // GET: /Admin/Users/Index
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
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

            // Areas/Admin/Views/Users/Index.cshtml
            return View(result);
        }

        // GET: /Admin/Users/SearchUser
        [HttpGet]
        public IActionResult SearchUser(string searchType, string searchKey, int page = 1, int pageSize = 10)
        {
            var usersQuery = _context.Users.AsQueryable();

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

            int totalItems = usersQuery.Count();
            var users = usersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!users.Any())
            {
                ViewBag.Message = "Không tìm thấy người dùng nào phù hợp với từ khóa tìm kiếm.";
            }

            var model = new UserSearchViewModel
            {
                Users = users,
                Pagination = new Paginate(totalItems, page, pageSize)
            };

            // Areas/Admin/Views/Users/_SearchUserResult.cshtml
            return PartialView("_SearchUserResult", model);
        }

        // GET: /Admin/Users/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Areas/Admin/Views/Users/Create.cshtml
            return View();
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.UserEmail == model.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email đã tồn tại.");
                    return View("Create", model); // <-- SỬA: Trả về View Create
                }

                // GHI CHÚ: Khởi tạo Controller khác ở đây là một "code smell"
                // Logic hash nên nằm trong 1 Service riêng (ví dụ: IPasswordHasher)
                // Nhưng mình giữ nguyên logic của bạn theo yêu cầu.
                var hashPass = new HashPasswordController();
                string hashedPass = hashPass.HashPassword(model.UserPassword);

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
                _context.Users.Add(user);
                _context.SaveChanges();

                var address = new AddressList
                {
                    IdUser = user.UserId,
                    AdrsFullname = user.UserFullname,
                    AdrsAddress = model.Address,
                    AdrsPhone = "N/A",
                    AdrsIsDefault = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.AddressLists.Add(address);
                _context.SaveChanges();

                return RedirectToAction("Index", "Users");
            }

            return View("Create", model);
        }

        // GET: /Admin/Users/Edit
        [HttpGet]
        public IActionResult Edit(long id)
        {

            var user = _context.Users
                .Where(u => u.UserId == id)
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

            // Areas/Admin/Views/Users/Edit.cshtml
            return View(viewModel);
        }

        // POST: /Admin/Users/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(long UserId, string UserEmail, string? UserNewPassword, string UserFullname, bool UserSex, DateOnly UserBirthday, bool UserIsActive, string UserRole)
        {
            // GHI CHÚ: Action này nên dùng ViewModel (EditUserViewModel) thay vì các tham số rời rạc
            // Nhưng mình giữ nguyên để không làm hỏng View của bạn.

            var user = _context.Users.FirstOrDefault(u => u.UserId == UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction("Index", "Users"); // <-- SỬA
            }

            user.UserEmail = UserEmail;
            user.UserFullname = UserFullname;
            user.UserSex = UserSex;
            user.UserBirthday = UserBirthday;
            user.UserIsActive = UserIsActive;
            user.UserRole = UserRole;

            if (!string.IsNullOrEmpty(UserNewPassword))
            {
                var hashPass = new HashPasswordController();
                user.UserPassword = hashPass.HashPassword(UserNewPassword);
            }

            try
            {
                user.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật thông tin người dùng thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật: " + ex.Message;
            }

            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        public async Task<IActionResult> Statistics(DateOnly? fromDate, DateOnly? toDate)
        {
            // --- 1. LẤY TOÀN BỘ DỮ LIỆU USER (CHO KPI VÀ BIỂU ĐỒ TRÒN) ---
            var allUsers = await _context.Users.ToListAsync();

            // Box: Tổng User
            int totalUsers = allUsers.Count;
            // Box: Tài khoản Active/Inactive
            int activeUsers = allUsers.Count(u => u.UserIsActive == true);
            int inactiveUsers = totalUsers - activeUsers;

            // Biểu đồ tròn: Trạng thái
            var statusLabels = new List<string> { "Hoạt động", "Bị khóa" };
            var statusData = new List<int> { activeUsers, inactiveUsers };
            var statusColors = new List<string> { "#28a745", "#dc3545" }; // Xanh lá, Đỏ

            // Biểu đồ tròn: Giới tính
            var genderQuery = allUsers
                .GroupBy(u => u.UserSex) // True = Nữ, False = Nam (Giả sử)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToList();

            var genderLabels = new List<string>();
            var genderData = new List<int>();
            var genderColors = new List<string> { "#007bff", "#e83e8c" }; // Xanh, Hồng

            foreach (var item in genderQuery)
            {
                genderLabels.Add(item.Key ? "Nữ" : "Nam"); // Dịch true/false
                genderData.Add(item.Count);
            }

            // --- 2. LẤY DỮ LIỆU BIỂU ĐỒ ĐƯỜNG (ĐĂNG KÝ MỚI) ---
            var today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly startDate = fromDate ?? today.AddDays(-29); // Mặc định 30 ngày qua
            DateOnly endDate = toDate ?? today;

            // Chuyển đổi DateOnly sang DateTime để so sánh (vì CreatedAt là DateTime)
            DateTime startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            DateTime endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var dailyRegistrations = await _context.Users
                .Where(u => u.CreatedAt >= startDateTime && u.CreatedAt <= endDateTime)
                .GroupBy(u => DateOnly.FromDateTime(u.CreatedAt)) // Nhóm theo Ngày
                .Select(g => new {
                    Day = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(r => r.Day, r => r.Count);

            var regChartLabels = new List<string>();
            var regChartData = new List<int>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                regChartLabels.Add(date.ToString("dd/MM"));
                regChartData.Add(dailyRegistrations.ContainsKey(date) ? dailyRegistrations[date] : 0);
            }

            // 3. TẠO VIEWMODEL
            var model = new UserStatisticsViewModel
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,

                RegistrationChartLabels = regChartLabels,
                RegistrationChartData = regChartData,

                GenderLabels = genderLabels,
                GenderData = genderData,
                GenderColors = genderColors,

                StatusLabels = statusLabels,
                StatusData = statusData,
                StatusColors = statusColors,

                FromDate = startDate,
                ToDate = endDate
            };

            return View(model);
        }
    }
}
