using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ALittleLeaf.Filters
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. Kiểm tra User đã được xác thực chưa (đã có JWT hợp lệ chưa)
            // User.Identity.IsAuthenticated sẽ true nếu JWT Middleware giải mã token thành công
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Chưa đăng nhập -> Chuyển hướng về trang Login
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        // Truyền thêm ReturnUrl để sau khi login quay lại đúng trang này
                        { "ReturnUrl", context.HttpContext.Request.Path }
                    });
            }
            base.OnActionExecuting(context);
        }
    }
}
