using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Product;
using ALittleLeaf.Api.Services.Product;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ALittleLeaf.Tests.Controllers
{
    public class CustomerProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController    _controller;

        public CustomerProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller  = new ProductsController(_mockService.Object);
        }

        // ── GET /api/products/{id} ────────────────────────────────────────────

        [Fact]
        public async Task GetProduct_ValidId_ReturnsOkWithDetail()
        {
            _mockService.Setup(s => s.GetProductDetailAsync(1))
                .ReturnsAsync(new ProductDetailDto { ProductId = 1, ProductName = "Ấm Tráng Men" });

            var result = await _controller.GetProduct(1);

            var dto = ApiAssert.OkValue<ProductDetailDto>(result);
            Assert.Equal(1, dto.ProductId);
        }

        [Fact]
        public async Task GetProduct_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetProductDetailAsync(999))
                .ReturnsAsync((ProductDetailDto?)null);

            var result = await _controller.GetProduct(999);

            ApiAssert.IsNotFound(result);
        }

        // ── GET /api/products (search / paginate) ─────────────────────────────

        [Fact]
        public async Task GetProducts_WithKeyword_ReturnsOkWithResults()
        {
            var mockResult = new PaginatedResultDto<ProductDto>
            {
                Items      = new List<ProductDto> { new() { ProductName = "Bếp Từ Cao Cấp" } },
                TotalItems = 1,
                Page       = 1,
                PageSize   = 12
            };
            _mockService.Setup(s => s.GetPaginatedProductsAsync(null, "Bếp", null, null, 1, 12))
                .ReturnsAsync(mockResult);

            var result = await _controller.GetProducts(keyword: "Bếp");

            var page = ApiAssert.OkValue<PaginatedResultDto<ProductDto>>(result);
            Assert.Single(page.Items);
        }

        [Fact]
        public async Task GetProducts_WithCategoryFilter_ReturnsOkWithFilteredResults()
        {
            var mockResult = new PaginatedResultDto<ProductDto>
            {
                Items      = new List<ProductDto> { new() { ProductName = "Cây Xương Rồng", IdCategory = 2 } },
                TotalItems = 1,
                Page       = 1,
                PageSize   = 12
            };
            _mockService.Setup(s => s.GetPaginatedProductsAsync(2, null, null, null, 1, 12))
                .ReturnsAsync(mockResult);

            var result = await _controller.GetProducts(categoryId: 2);

            var page = ApiAssert.OkValue<PaginatedResultDto<ProductDto>>(result);
            Assert.All(page.Items, p => Assert.Equal(2, p.IdCategory));
        }

        // ── GET /api/products/{id}/related ────────────────────────────────────

        [Fact]
        public async Task GetRelated_ValidId_ReturnsOkWithList()
        {
            _mockService.Setup(s => s.GetProductDetailAsync(1))
                .ReturnsAsync(new ProductDetailDto { ProductId = 1 });
            _mockService.Setup(s => s.GetRelatedProductsAsync(1, 4))
                .ReturnsAsync(new List<ProductDto> { new() { ProductId = 2 }, new() { ProductId = 3 } });
            var result = await _controller.GetRelated(1);

            var list = ApiAssert.OkValue<List<ProductDto>>(result);
            Assert.Equal(2, list.Count);

        }

        [Fact]
        public async Task GetRelated_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetProductDetailAsync(999))
                .ReturnsAsync((ProductDetailDto?)null);

            var result = await _controller.GetRelated(999);

            ApiAssert.IsNotFound(result);
        }
    }
}
