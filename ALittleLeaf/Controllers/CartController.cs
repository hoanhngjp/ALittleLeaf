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
        // CHỈ CẦN 2 SERVICE NÀY
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
