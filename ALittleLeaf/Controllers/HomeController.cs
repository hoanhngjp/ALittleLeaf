using ALittleLeaf.Models;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ALittleLeaf.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            ViewData["Cart"] = cart;

            return View();
        }
    }
}
