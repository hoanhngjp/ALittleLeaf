using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SiteNavController : Controller
{
    private readonly DataContext _context;

    public SiteNavController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, int>>("cart");
        var model = new SiteNavViewModel();

        // Xử lý giỏ hàng
        if (cart != null && cart.Any())
        {
            foreach (var item in cart)
            {
                var product = _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefault(p => p.ProductId == item.Key);

                if (product != null)
                {
                    var quantity = item.Value;
                    model.TotalPrice += product.ProductPrice * quantity;

                    model.CartItems.Add(new CartItemViewModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ImgName = product.ProductImages.FirstOrDefault()?.ImgName,
                        Quantity = quantity,
                        ProductPrice = product.ProductPrice
                    });
                }
            }
        }

        return PartialView("~/Views/Shared/_SiteNav.cshtml", model); // Trả về Partial View với model SiteNav
    }
}
