using ALittleLeaf.Services.Cart;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ALittleLeaf.Controllers
{
    public class SiteBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cartService = context.HttpContext.RequestServices.GetService<ICartService>();

            if (cartService != null)
            {
                ViewData["Cart"] = cartService.GetCartItems();
                ViewData["CartItemCount"] = cartService.GetCartItemCount();
                ViewData["CartTotalPrice"] = cartService.GetCartTotal();
            }

            base.OnActionExecuting(context);
        }
    }
}
