using ALittleLeaf.Api.DTOs.Banner;
using ALittleLeaf.Api.Services.Banner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers.Admin
{
    /// <summary>
    /// Admin banner management. All actions require the "admin" role.
    /// Image uploads use multipart/form-data (POST only).
    /// </summary>
    [ApiController]
    [Route("api/admin/banners")]
    [Authorize(Roles = "admin")]
    public class AdminBannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public AdminBannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        // ── GET /api/admin/banners ────────────────────────────────────────────

        /// <summary>Returns all banners (active and inactive) for the admin panel.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BannerAdminDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBanners()
        {
            var banners = await _bannerService.GetAllBannersAsync();
            return Ok(banners);
        }

        // ── POST /api/admin/banners ───────────────────────────────────────────

        /// <summary>
        /// Uploads a new banner image to Cloudinary and creates a DB record.
        /// Accepts multipart/form-data. Max file size: 5 MB.
        /// Allowed MIME types: image/jpeg, image/png, image/webp.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BannerAdminDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBanner([FromForm] CreateBannerDto dto)
        {
            try
            {
                var created = await _bannerService.CreateBannerAsync(dto);
                return CreatedAtAction(nameof(GetAllBanners), new { id = created.BannerId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── PUT /api/admin/banners/{id} ───────────────────────────────────────

        /// <summary>
        /// Updates banner metadata (IsActive, DisplayOrder, TargetUrl).
        /// Only non-null fields are applied — image re-upload is not supported via this endpoint.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BannerAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBanner(int id, [FromBody] UpdateBannerDto dto)
        {
            var updated = await _bannerService.UpdateBannerAsync(id, dto);
            if (updated is null) return NotFound(new { error = $"Banner {id} not found." });
            return Ok(updated);
        }

        // ── DELETE /api/admin/banners/{id} ────────────────────────────────────

        /// <summary>
        /// Deletes the banner from both the database and Cloudinary.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var deleted = await _bannerService.DeleteBannerAsync(id);
            if (!deleted) return NotFound(new { error = $"Banner {id} not found." });
            return NoContent();
        }
    }
}
