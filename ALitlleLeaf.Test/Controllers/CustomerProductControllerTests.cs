using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ALittleLeaf.Controllers;
using ALittleLeaf.Services.Product;
using ALittleLeaf.ViewModels;
using ALittleLeaf.Models;

namespace ALittleLeaf.Tests.Controllers
{
    public class CustomerProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;

        public CustomerProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
        }

        // --- PRODUCT CONTROLLER (Xem chi tiết) ---

        [Fact]
        public async Task Product_Index_ValidId_ReturnsView()
        {
            // Arrange
            var controller = new ProductController(_mockService.Object);
            _mockService.Setup(s => s.GetProductDetailAsync(1))
                        .ReturnsAsync(new ProductDetailViewModel { ProductId = 1 });

            // Act
            var result = await controller.Index(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductDetailViewModel>(viewResult.Model);
            Assert.Equal(1, model.ProductId);
        }

        [Fact]
        public async Task Product_Index_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var controller = new ProductController(_mockService.Object);
            _mockService.Setup(s => s.GetProductDetailAsync(999))
                        .ReturnsAsync((ProductDetailViewModel)null);

            // Act
            var result = await controller.Index(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // --- SEARCH CONTROLLER (Tìm kiếm) ---

        [Fact]
        public async Task Search_Index_WithKeyword_ReturnsResults()
        {
            // Arrange
            var controller = new SearchController(_mockService.Object);
            var mockResult = new ProductServiceResult
            {
                Products = new List<ProductViewModel> { new ProductViewModel { ProductName = "Bếp" } },
                Pagination = new Paginate(1, 1, 10)
            };

            _mockService.Setup(s => s.GetPaginatedProductsAsync(null, "Bếp", null, null, 1, 15))
                        .ReturnsAsync(mockResult);

            // Act
            var result = await controller.Index("Bếp");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ProductViewModel>>(viewResult.Model);
            Assert.Single(model);
        }

        // --- COLLECTIONS CONTROLLER (Lọc) ---

        [Fact]
        public async Task Collections_Index_WithFilter_ReturnsView()
        {
            // Arrange
            var controller = new CollectionsController(_mockService.Object);
            var mockResult = new ProductServiceResult
            {
                Products = new List<ProductViewModel>(),
                Pagination = new Paginate(0, 1, 12),
                PageTitle = "Bộ lọc"
            };

            _mockService.Setup(s => s.GetPaginatedProductsAsync(1, null, 100, 200, 1, 12))
                        .ReturnsAsync(mockResult);

            // Act
            var result = await controller.Index(1, 100, 200);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Bộ lọc", controller.ViewBag.CategoryName);
            Assert.Equal(100, controller.ViewBag.MinPrice);
        }
    }
}