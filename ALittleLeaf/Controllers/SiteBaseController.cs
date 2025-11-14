using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ALittleLeaf.Controllers
{
    public class SiteBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Tự động tải giỏ hàng từ Session
            var cart = context.HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart")
                       ?? new List<CartItemViewModel>();

            // Tự động gán nó vào ViewData
            ViewData["Cart"] = cart;

            // Chạy tiếp các logic khác
            base.OnActionExecuting(context);
        }
    }
}
