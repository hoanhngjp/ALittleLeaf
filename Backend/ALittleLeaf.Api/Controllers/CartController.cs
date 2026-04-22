using System.Security.Claims;
using ALittleLeaf.Api.DTOs.Cart;
using ALittleLeaf.Api.Services.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// DB-backed cart endpoints. All routes require a valid JWT.
    ///
    /// SECURITY: The userId is NEVER accepted from the request body or URL params.
    /// It is always extracted server-side from the JWT claim
    /// <see cref="ClaimTypes.NameIdentifier"/> set by <c>AuthService.IssueTokenPairAsync</c>.
    /// </summary>
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Extracts the userId from the JWT. Returns 0 on failure (caller must check).
        /// </summary>
        private bool TryGetUserId(out long userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(raw, out userId);
        }

        // ── GET /api/cart ─────────────────────────────────────────────────────

        /// <summary>
        /// Returns the current user's cart (creates an empty one on first access).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCart()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        // ── POST /api/cart/items ──────────────────────────────────────────────

        /// <summary>
        /// Adds a product to the cart. If the product is already in the cart,
        /// its quantity is incremented rather than duplicated.
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var cart = await _cartService.AddItemAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(cart);
        }

        // ── PUT /api/cart/items/{productId} ───────────────────────────────────

        /// <summary>Sets the quantity of an existing cart item to an exact value.</summary>
        [HttpPut("items/{productId:int}")]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(int productId, [FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var cart = await _cartService.UpdateItemAsync(userId, productId, dto.Quantity);
            if (cart == null)
                return NotFound(new { error = $"Product {productId} is not in your cart." });

            return Ok(cart);
        }

        // ── DELETE /api/cart/items/{productId} ────────────────────────────────

        /// <summary>Removes a single product line from the cart.</summary>
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var cart = await _cartService.RemoveItemAsync(userId, productId);
            if (cart == null)
                return NotFound(new { error = $"Product {productId} is not in your cart." });

            return Ok(cart);
        }

        // ── DELETE /api/cart ──────────────────────────────────────────────────

        /// <summary>Removes all items from the cart (empty cart is returned).</summary>
        [HttpDelete]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var cart = await _cartService.ClearCartAsync(userId);
            return Ok(cart);
        }
    }
}
