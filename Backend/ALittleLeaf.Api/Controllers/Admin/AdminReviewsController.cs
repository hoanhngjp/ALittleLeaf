using ALittleLeaf.Api.Services.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/reviews")]
    [Authorize(Roles = "admin")]
    public class AdminReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public AdminReviewsController(IReviewService service)
        {
            _service = service;
        }

        /// <summary>List all reviews with optional productId filter.</summary>
        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] int? productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllReviewsAsync(productId, page, pageSize);
            return Ok(result);
        }

        /// <summary>Hard-delete a review.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var deleted = await _service.DeleteReviewAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
