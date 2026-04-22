using ALittleLeaf.Api.Controllers.Admin;
using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Services.Admin;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ALittleLeaf.Tests.Controllers
{
    /// <summary>
    /// Tests for AdminProductsController.
    /// Image uploads now go through IAdminService → ICloudinaryService.
    /// No IWebHostEnvironment — Cloudinary is handled entirely in the service layer.
    /// </summary>
    public class AdminProductControllerTests
    {
        private readonly Mock<IAdminService>     _mockAdminService;
        private readonly AdminProductsController _controller;

        public AdminProductControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller       = new AdminProductsController(_mockAdminService.Object);
        }

        // GET /api/admin/products — returns paginated list
        [Fact]
        public async Task GetProducts_ReturnsOkWithPagedResult()
        {
            var paged = new PaginatedAdminResultDto<AdminProductDto>
            {
                Items      = new List<AdminProductDto>
                {
                    new() { ProductId = 1, ProductName = "Ấm Tráng Men" },
                    new() { ProductId = 2, ProductName = "Bếp Từ Cao Cấp" }
                },
                TotalItems = 2,
                Page       = 1,
                PageSize   = 20
            };
            _mockAdminService
                .Setup(s => s.GetProductsAsync(null, null, null, "createdAt", true, 1, 20))
                .ReturnsAsync(paged);

            var result = await _controller.GetProducts(null, null, null);

            var dto = ApiAssert.OkValue<PaginatedAdminResultDto<AdminProductDto>>(result);
            Assert.Equal(2, dto.Items.Count());
        }

        // GET /api/admin/products/{id} — found
        [Fact]
        public async Task GetProduct_ValidId_ReturnsOkWithDetail()
        {
            var product = new AdminProductDto { ProductId = 1, ProductName = "Ấm Tráng Men" };
            _mockAdminService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.GetProduct(1);

            var dto = ApiAssert.OkValue<AdminProductDto>(result);
            Assert.Equal(1, dto.ProductId);
        }

        // GET /api/admin/products/{id} — not found → 404
        [Fact]
        public async Task GetProduct_InvalidId_ReturnsNotFound()
        {
            _mockAdminService.Setup(s => s.GetProductByIdAsync(999))
                .ReturnsAsync((AdminProductDto?)null);

            var result = await _controller.GetProduct(999);

            ApiAssert.IsNotFound(result);
        }

        // POST /api/admin/products — TC-01: create product → 201 Created
        // Cloudinary upload is handled inside IAdminService.CreateProductAsync;
        // the controller only passes the DTO — no file system involved here.
        [Fact]
        public async Task CreateProduct_ValidDto_Returns201WithCreatedProduct()
        {
            var dto = new CreateProductDto
            {
                ProductName     = "New Plant",
                ProductPrice    = 80000,
                IdCategory      = 2,
                QuantityInStock = 15,
                IsOnSale        = true
            };
            var created = new AdminProductDto
            {
                ProductId   = 10,
                ProductName = "New Plant",
                ProductPrice = 80000
            };
            _mockAdminService.Setup(s => s.CreateProductAsync(dto)).ReturnsAsync(created);

            var result = await _controller.CreateProduct(dto);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result);
            var product   = Assert.IsType<AdminProductDto>(createdAt.Value);
            Assert.Equal(10, product.ProductId);
            Assert.Equal("New Plant", product.ProductName);
        }

        // PUT /api/admin/products/{id} — TC-04: edit product → 200 OK
        [Fact]
        public async Task UpdateProduct_ValidId_ReturnsOkWithUpdatedProduct()
        {
            var dto = new UpdateProductDto { ProductName = "Updated Name", ProductPrice = 99999 };
            var updated = new AdminProductDto { ProductId = 1, ProductName = "Updated Name", ProductPrice = 99999 };
            _mockAdminService.Setup(s => s.UpdateProductAsync(1, dto)).ReturnsAsync(updated);

            var result = await _controller.UpdateProduct(1, dto);

            var product = ApiAssert.OkValue<AdminProductDto>(result);
            Assert.Equal("Updated Name", product.ProductName);
            Assert.Equal(99999, product.ProductPrice);
        }

        // PUT /api/admin/products/{id} — not found → 404
        [Fact]
        public async Task UpdateProduct_InvalidId_ReturnsNotFound()
        {
            var dto = new UpdateProductDto { ProductName = "X" };
            _mockAdminService.Setup(s => s.UpdateProductAsync(999, dto))
                .ReturnsAsync((AdminProductDto?)null);

            var result = await _controller.UpdateProduct(999, dto);

            ApiAssert.IsNotFound(result);
        }

        // DELETE /api/admin/products/{id} — TC-05: soft/hard delete → 204 NoContent
        [Fact]
        public async Task DeleteProduct_ValidId_Returns204()
        {
            _mockAdminService.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteProduct(1);

            Assert.IsType<NoContentResult>(result);
            _mockAdminService.Verify(s => s.DeleteProductAsync(1), Times.Once);
        }

        // DELETE /api/admin/products/{id} — not found → 404
        [Fact]
        public async Task DeleteProduct_InvalidId_ReturnsNotFound()
        {
            _mockAdminService.Setup(s => s.DeleteProductAsync(999)).ReturnsAsync(false);

            var result = await _controller.DeleteProduct(999);

            ApiAssert.IsNotFound(result);
        }
    }
}
