using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("AdminId")))
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { Area = "Admin" });
            }
            base.OnActionExecuting(context);
        }
    }
}
