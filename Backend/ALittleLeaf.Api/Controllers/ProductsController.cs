using ALittleLeaf.Api.DTOs.Product;
using ALittleLeaf.Api.Services.Product;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Public read-only product endpoints consumed by the React SPA.
    /// No authentication required — all products listed here are already IsOnSale = true.
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // ── GET /api/products ─────────────────────────────────────────────────

        /// <summary>
        /// Returns a paginated list of on-sale products, optionally filtered by category,
        /// keyword, or price range.
        /// </summary>
        /// <param name="categoryId">Filter by category (parent IDs expand to their children).</param>
        /// <param name="keyword">Full-text keyword filter (applied when categoryId is absent).</param>
        /// <param name="minPrice">Inclusive lower price bound.</param>
        /// <param name="maxPrice">Inclusive upper price bound.</param>
        /// <param name="page">1-based page number (default 1).</param>
        /// <param name="pageSize">Items per page (default 12, max 50).</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResultDto<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int?    categoryId = null,
            [FromQuery] string? keyword    = null,
            [FromQuery] int?    minPrice   = null,
            [FromQuery] int?    maxPrice   = null,
            [FromQuery] int     page       = 1,
            [FromQuery] int     pageSize   = 12)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);
            page     = Math.Max(page, 1);

            var result = await _productService.GetPaginatedProductsAsync(
                categoryId, keyword, minPrice, maxPrice, page, pageSize);

            return Ok(result);
        }

        // ── GET /api/products/search ──────────────────────────────────────────

        /// <summary>
        /// Live-search endpoint: returns up to 5 lightweight products whose name contains
        /// the supplied keyword. Intended for typeahead / autocomplete UI.
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { error = "Keyword is required." });

            var results = await _productService.SearchLiveAsync(keyword);
            return Ok(results);
        }

        // ── GET /api/products/{id} ────────────────────────────────────────────

        /// <summary>Returns the full detail of a single product including all images.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductDetailAsync(id);
            if (product == null)
                return NotFound(new { error = $"Product {id} not found." });

            return Ok(product);
        }

        // ── GET /api/products/{id}/related ────────────────────────────────────

        /// <summary>Returns up to 4 products from the same category (excludes the source product).</summary>
        [HttpGet("{id:int}/related")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRelated(int id)
        {
            var product = await _productService.GetProductDetailAsync(id);
            if (product == null)
                return NotFound(new { error = $"Product {id} not found." });

            var related = await _productService.GetRelatedProductsAsync(id);
            return Ok(related);
        }
    }
}
