using ALittleLeaf.Models;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ALittleLeaf.Controllers
{
    public class HomeController : SiteBaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
