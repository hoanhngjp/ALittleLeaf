using System.Security.Claims;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Order management endpoints. All routes require a valid JWT.
    /// userId is ALWAYS extracted server-side from the JWT — never from the request body.
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private bool TryGetUserId(out long userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(raw, out userId);
        }

        // ── POST /api/orders ──────────────────────────────────────────────────

        /// <summary>
        /// Places an order from the current user's DB cart.
        /// For COD: stock is decremented and cart is cleared immediately.
        /// For VNPAY: bill is saved as "pending_vnpay"; use POST /api/payment/create-url next.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, dto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.BillId }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── GET /api/orders ───────────────────────────────────────────────────

        /// <summary>Returns the order history for the authenticated user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrders()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var orders = await _orderService.GetOrderHistoryAsync(userId);
            return Ok(orders);
        }

        // ── GET /api/orders/{id} ──────────────────────────────────────────────

        /// <summary>Returns a single order with full line-item details.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var order = await _orderService.GetOrderDetailAsync(id, userId);
            if (order == null)
                return NotFound(new { error = $"Order {id} not found." });

            return Ok(order);
        }
    }
}
