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

        public IActionResult Collections()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
