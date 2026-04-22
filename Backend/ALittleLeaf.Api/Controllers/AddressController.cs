using System.Security.Claims;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Address book management. All routes require a valid JWT.
    /// userId is ALWAYS extracted server-side from the JWT.
    /// </summary>
    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AddressController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private bool TryGetUserId(out long userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(raw, out userId);
        }

        // ── GET /api/addresses ────────────────────────────────────────────────

        /// <summary>Returns all saved addresses for the authenticated user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAddresses()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var addresses = await _orderService.GetUserAddressesAsync(userId);
            return Ok(addresses);
        }

        // ── GET /api/addresses/{id} ───────────────────────────────────────────

        /// <summary>Returns a single address.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAddress(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var address = await _orderService.GetAddressDetailAsync(id);
            if (address == null || address.AdrsId == 0)
                return NotFound(new { error = $"Address {id} not found." });

            return Ok(address);
        }

        // ── POST /api/addresses ───────────────────────────────────────────────

        /// <summary>Creates a new address for the authenticated user.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var address = await _orderService.AddAddressAsync(userId, dto);
            return CreatedAtAction(nameof(GetAddress), new { id = address.AdrsId }, address);
        }

        // ── PUT /api/addresses/{id} ───────────────────────────────────────────

        /// <summary>Updates an existing address owned by the authenticated user.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var address = await _orderService.UpdateAddressAsync(userId, id, dto);
            if (address == null)
                return NotFound(new { error = $"Address {id} not found or does not belong to you." });

            return Ok(address);
        }

        // ── DELETE /api/addresses/{id} ────────────────────────────────────────

        /// <summary>Deletes an address owned by the authenticated user.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var deleted = await _orderService.DeleteAddressAsync(userId, id);
            if (!deleted)
                return NotFound(new { error = $"Address {id} not found or does not belong to you." });

            return NoContent();
        }
    }
}
