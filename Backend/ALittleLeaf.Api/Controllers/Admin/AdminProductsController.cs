using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers.Admin
{
    /// <summary>
    /// Admin product management. All actions require the "admin" role.
    /// Image uploads use multipart/form-data.
    /// </summary>
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "admin")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminProductsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── GET /api/admin/products ───────────────────────────────────────────

        /// <summary>
        /// Paginated, searchable, sortable product list.
        /// sortBy: productName | productPrice | quantityInStock | createdAt | updatedAt
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedAdminResultDto<AdminProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? keyword,
            [FromQuery] int?    categoryId,
            [FromQuery] bool?   isOnSale,
            [FromQuery] string? sortBy       = "createdAt",
            [FromQuery] bool    isDescending = true,
            [FromQuery] int     page         = 1,
            [FromQuery] int     pageSize     = 20)
        {
            var result = await _adminService.GetProductsAsync(
                keyword, categoryId, isOnSale, sortBy, isDescending, page, pageSize);
            return Ok(result);
        }

        // ── GET /api/admin/products/{id} ──────────────────────────────────────

        /// <summary>Returns a single product by ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AdminProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _adminService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { error = $"Product {id} not found." });
            return Ok(product);
        }

        // ── POST /api/admin/products ──────────────────────────────────────────

        /// <summary>Creates a new product. Accepts multipart/form-data for image uploads.</summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _adminService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // ── PUT /api/admin/products/{id} ──────────────────────────────────────

        /// <summary>Updates an existing product. Accepts multipart/form-data for image changes.</summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _adminService.UpdateProductAsync(id, dto);
            if (product == null) return NotFound(new { error = $"Product {id} not found." });
            return Ok(product);
        }

        // ── DELETE /api/admin/products/{id} ───────────────────────────────────

        /// <summary>Deletes a product and all its Cloudinary images.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _adminService.DeleteProductAsync(id);
            if (!deleted) return NotFound(new { error = $"Product {id} not found." });
            return NoContent();
        }
    }
}
