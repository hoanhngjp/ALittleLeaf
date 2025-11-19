using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ALittleLeaf.Filters
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy session
            var session = context.HttpContext.Session;

            // Chạy logic check login của bạn
            if (string.IsNullOrEmpty(session.GetString("UserId")))
            {
                // Nếu chưa đăng nhập, đá về trang Login
                // (Đây là cách Redirect bên trong một Attribute)
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" }
                    });
            }

            base.OnActionExecuting(context);
        }
    }
}
