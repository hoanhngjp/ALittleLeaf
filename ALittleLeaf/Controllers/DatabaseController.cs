using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class DatabaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
