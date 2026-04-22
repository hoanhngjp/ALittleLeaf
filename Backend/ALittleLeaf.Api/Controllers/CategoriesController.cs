using ALittleLeaf.Api.DTOs.Product;
using ALittleLeaf.Api.Services.Category;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Endpoints for category navigation and category-scoped product listings.
    /// </summary>
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // ── GET /api/categories ───────────────────────────────────────────────

        /// <summary>
        /// Returns all root categories, each with their sub-categories nested inside.
        /// Used to build the site navigation menu.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // ── GET /api/categories/{name}/products ───────────────────────────────

        /// <summary>
        /// Returns a paginated product list for the category identified by its name.
        /// Supports optional price-range filtering.
        /// </summary>
        /// <param name="name">Category name (URL-encoded, case-insensitive).</param>
        /// <param name="minPrice">Inclusive lower price bound.</param>
        /// <param name="maxPrice">Inclusive upper price bound.</param>
        /// <param name="page">1-based page number (default 1).</param>
        /// <param name="pageSize">Items per page (default 12, max 50).</param>
        [HttpGet("{name}/products")]
        [ProducesResponseType(typeof(PaginatedResultDto<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsByCategory(
            string          name,
            [FromQuery] int?    minPrice = null,
            [FromQuery] int?    maxPrice = null,
            [FromQuery] int     page     = 1,
            [FromQuery] int     pageSize = 12)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);
            page     = Math.Max(page, 1);

            var result = await _categoryService.GetProductsByCategoryNameAsync(
                name, minPrice, maxPrice, page, pageSize);

            if (result == null)
                return NotFound(new { error = $"Category '{name}' not found." });

            return Ok(result);
        }
    }
}
