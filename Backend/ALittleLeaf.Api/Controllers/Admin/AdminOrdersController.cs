using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers.Admin
{
    /// <summary>
    /// Admin order management. All actions require the "admin" role.
    /// </summary>
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminOrdersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── GET /api/admin/orders ─────────────────────────────────────────────

        /// <summary>Paginated order list with optional status filters.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedAdminResultDto<AdminOrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? keyword        = null,
            [FromQuery] string? orderStatus    = null,
            [FromQuery] string? shippingStatus = null,
            [FromQuery] string? paymentStatus  = null,
            [FromQuery] string? startDate      = null,
            [FromQuery] string? endDate        = null,
            [FromQuery] string? sortBy         = "dateCreated",
            [FromQuery] bool    isDescending   = true,
            [FromQuery] int     page           = 1,
            [FromQuery] int     pageSize       = 10)
        {
            DateOnly? start = DateOnly.TryParseExact(startDate, "yyyy-MM-dd", out var sd) ? sd : null;
            DateOnly? end   = DateOnly.TryParseExact(endDate,   "yyyy-MM-dd", out var ed) ? ed : null;

            var result = await _adminService.GetOrdersAsync(
                keyword, orderStatus, shippingStatus, paymentStatus,
                start, end,
                sortBy, isDescending,
                page, pageSize);
            return Ok(result);
        }

        // ── GET /api/admin/orders/{id} ────────────────────────────────────────

        /// <summary>Returns full order detail including line items.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AdminOrderDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _adminService.GetOrderByIdAsync(id);
            if (order == null) return NotFound(new { error = $"Order {id} not found." });
            return Ok(order);
        }

        // ── POST /api/admin/orders/{id}/confirm ──────────────────────────────

        /// <summary>Admin confirms an order (PENDING → CONFIRMED). Required before GHN push.</summary>
        [HttpPost("{id:int}/confirm")]
        [ProducesResponseType(typeof(AdminOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            try
            {
                var order = await _adminService.ConfirmOrderAsync(id);
                if (order == null) return NotFound(new { error = $"Order {id} not found." });
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── POST /api/admin/orders/{id}/sync-ghn ─────────────────────────────

        /// <summary>
        /// Manually pushes a paid order to GHN and stores the returned order code.
        /// No-op if the order already has a GHN code.
        /// </summary>
        [HttpPost("{id:int}/sync-ghn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SyncToGhn(int id)
        {
            try
            {
                var code = await _adminService.SyncOrderToGhnAsync(id);
                if (code == null) return NotFound(new { error = $"Order {id} not found." });
                return Ok(new { ghnOrderCode = code });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── PATCH /api/admin/orders/{id}/status ───────────────────────────────

        /// <summary>
        /// Updates the shipping status, payment status, and/or confirmed flag of an order.
        /// At least one field must be provided.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(typeof(AdminOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (dto.ShippingStatus == null && dto.PaymentStatus == null && dto.IsConfirmed == null)
                return BadRequest(new { error = "At least one status field must be provided." });

            var order = await _adminService.UpdateOrderStatusAsync(id, dto);
            if (order == null) return NotFound(new { error = $"Order {id} not found." });
            return Ok(order);
        }
    }
}
