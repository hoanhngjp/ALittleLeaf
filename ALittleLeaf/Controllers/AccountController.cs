using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
