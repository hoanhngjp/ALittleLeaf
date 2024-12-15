using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminThongKeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
