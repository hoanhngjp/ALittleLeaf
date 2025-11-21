using ALittleLeaf.Filters;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Cart;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;


namespace ALittleLeaf.Controllers
{
    public class CartController : SiteBaseController
    {
        private readonly ICartService _cartService;
        private readonly AlittleLeafDecorContext _context;

        public CartController(ICartService cartService, AlittleLeafDecorContext context)
        {
            _cartService = cartService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [CheckLogin]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            
            var product = await _context.Products.FindAsync(productId);

            if (product == null) return NotFound();

            if (quantity > product.QuantityInStock)
            {
                TempData["Error"] = $"Kho chỉ còn {product.QuantityInStock} sản phẩm. Vui lòng chọn lại số lượng.";

                return RedirectToAction("Index", "Product", new { productId = productId, idCategory = product.IdCategory });
            }

            var result = await _cartService.AddToCartAsync(productId, quantity);

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0" });
            }

            var product = _context.Products.Find(productId);
            if (product != null && quantity > product.QuantityInStock)
            {
                // Trả về JSON lỗi để Javascript bên giỏ hàng hiển thị alert
                return Json(new
                {
                    success = false,
                    message = $"Kho chỉ còn {product.QuantityInStock} sản phẩm."
                });
            }

            var result = _cartService.UpdateCartItem(productId, quantity);

            return Json(result);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var result = _cartService.RemoveFromCart(productId);

            return Json(result);
        }

        [CheckLogin]
        [HttpPost]
        public IActionResult GetCartNote(string? BillNote)
        {
            _cartService.SaveCartNote(BillNote);
            return RedirectToAction("Index", "Checkout");
        }
    }
}
