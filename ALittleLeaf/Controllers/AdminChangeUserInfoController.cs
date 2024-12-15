using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
                    UserPassword = u.UserPassword,
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
    }
}
