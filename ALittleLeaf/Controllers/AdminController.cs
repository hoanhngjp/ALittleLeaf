using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            int totalUsers = _context.Users.Count();
            int totalProducts = _context.Products.Count();
            int totalBills = _context.Bills.Count();

            var adminViewModel = new AdminViewModel
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalBills = totalBills
            };                
                
            return View(adminViewModel);
        }
    }
}
