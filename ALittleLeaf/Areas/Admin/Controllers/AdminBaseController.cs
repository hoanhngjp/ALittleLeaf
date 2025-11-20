using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "area", "Admin" },
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "ReturnUrl", context.HttpContext.Request.Path }
                    });
                return;
            }

            var role = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
            {
                context.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                        { "area", "Admin" },
                        { "controller", "Account" },
                        { "action", "Login" }
                   });
            }

            base.OnActionExecuting(context);
        }
    }
}
