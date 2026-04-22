using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers.Admin
{
    /// <summary>
    /// Admin user management. All actions require the "admin" role.
    /// </summary>
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminUsersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── GET /api/admin/users ──────────────────────────────────────────────

        /// <summary>Paginated user list with keyword, role, sex, and active-status filters.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedAdminResultDto<AdminUserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? keyword      = null,
            [FromQuery] bool?   isActive     = null,
            [FromQuery] string? userRole     = null,
            [FromQuery] bool?   userSex      = null,
            [FromQuery] string? sortBy       = "createdAt",
            [FromQuery] bool    isDescending = true,
            [FromQuery] int     page         = 1,
            [FromQuery] int     pageSize     = 10)
        {
            var result = await _adminService.GetUsersAsync(
                keyword, isActive, userRole, userSex,
                sortBy, isDescending, page, pageSize);
            return Ok(result);
        }

        // ── POST /api/admin/users ─────────────────────────────────────────────

        /// <summary>Creates a new user account. Password is hashed server-side.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (user, error) = await _adminService.CreateUserAsync(dto);
            if (error != null) return BadRequest(new { error });
            return CreatedAtAction(nameof(GetUser), new { id = user!.UserId }, user);
        }

        // ── GET /api/admin/users/{id} ─────────────────────────────────────────

        /// <summary>Returns a single user by ID.</summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { error = $"User {id} not found." });
            return Ok(user);
        }

        // ── PUT /api/admin/users/{id} ─────────────────────────────────────────

        /// <summary>
        /// Updates a user's profile, active status, or role.
        /// All fields are optional — only provided fields are changed.
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] AdminUpdateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _adminService.UpdateUserAsync(id, dto);
            if (user == null) return NotFound(new { error = $"User {id} not found." });
            return Ok(user);
        }

        // ── DELETE /api/admin/users/{id} ──────────────────────────────────────

        /// <summary>
        /// Deactivates a user account (soft delete — sets UserIsActive = false).
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateUser(long id)
        {
            var user = await _adminService.UpdateUserAsync(id, new AdminUpdateUserDto { UserIsActive = false });
            if (user == null) return NotFound(new { error = $"User {id} not found." });
            return NoContent();
        }
    }
}
