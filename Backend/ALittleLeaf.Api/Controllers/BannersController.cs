using ALittleLeaf.Api.DTOs.Banner;
using ALittleLeaf.Api.Services.Banner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Public banner feed — no authentication required.
    /// Returns at most 5 active banners ordered by DisplayOrder (enforced in the repository).
    /// </summary>
    [ApiController]
    [Route("api/banners")]
    [AllowAnonymous]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        /// <summary>Returns active banners for the homepage slider.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BannerDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveBanners()
        {
            var banners = await _bannerService.GetActiveBannersAsync();
            return Ok(banners);
        }
    }
}
