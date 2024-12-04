using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
