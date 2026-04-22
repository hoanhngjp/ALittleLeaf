using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers.Admin
{
    /// <summary>
    /// Admin dashboard stats. Requires the "admin" role.
    /// </summary>
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminDashboardController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── GET /api/admin/dashboard ──────────────────────────────────────────

        /// <summary>
        /// Returns aggregated stats filtered by optional date range.
        /// Defaults to the last 12 months when no dates are provided.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate   = null)
        {
            var dashboard = await _adminService.GetDashboardAsync(startDate, endDate);
            return Ok(dashboard);
        }
    }
}
