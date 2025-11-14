using ALittleLeaf.Filters;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    [CheckLogin]
    public class AccountController : SiteBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
