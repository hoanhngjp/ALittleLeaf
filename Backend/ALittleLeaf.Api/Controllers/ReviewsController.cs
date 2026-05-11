using System.Security.Claims;
using ALittleLeaf.Api.DTOs.Review;
using ALittleLeaf.Api.Services.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        /// <summary>Paginated reviews for a product.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews([FromQuery] int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (productId <= 0) return BadRequest("productId is required.");
            var result = await _service.GetProductReviewsAsync(productId, page, pageSize);
            return Ok(result);
        }

        /// <summary>Average rating + review count for a product.</summary>
        [HttpGet("rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRating([FromQuery] int productId)
        {
            if (productId <= 0) return BadRequest("productId is required.");
            var result = await _service.GetProductRatingAsync(productId);
            return Ok(result);
        }

        /// <summary>Check whether the current user has already reviewed this product.</summary>
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReview([FromQuery] int productId)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var review = await _service.GetMyReviewAsync(userId, productId);
            return Ok(review);
        }

        /// <summary>Submit a review. User must have a completed order containing the product.</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var review = await _service.CreateReviewAsync(userId, dto);
                return CreatedAtAction(nameof(GetReviews), new { productId = dto.ProductId }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
