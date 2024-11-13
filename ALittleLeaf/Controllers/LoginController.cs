using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View();
        }
    }
}
