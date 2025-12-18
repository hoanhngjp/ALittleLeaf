using ALittleLeaf.Areas.Admin.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Hosting; // Cần cài package Microsoft.AspNetCore.Hosting.Abstractions
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Controllers
{
    public class AdminProductControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly ProductsController _controller;
        private readonly string _tempPath;

        public AdminProductControllerTests()
        {
            // 1. Setup DB
            _context = DbContextFactory.Create();

            // 2. Setup File Environment (Giả lập thư mục upload)
            _mockEnv = new Mock<IWebHostEnvironment>();

            // Tạo thư mục tạm để test việc lưu file
            _tempPath = Path.Combine(Path.GetTempPath(), "ALittleLeafTest_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempPath);
            _mockEnv.Setup(e => e.WebRootPath).Returns(_tempPath);

            _controller = new ProductsController(_context, _mockEnv.Object);
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
            // Xóa thư mục tạm
            if (Directory.Exists(_tempPath)) Directory.Delete(_tempPath, true);
        }

        // TC-01: Thêm sản phẩm thành công
        [Fact]
        public void Create_Post_ValidModel_AddsProductToDb()
        {
            // Arrange
            var model = new AddProductViewModel
            {
                ProductName = "Admin Product",
                ProductPrice = 50000,
                IdCategory = 1,
                ProductDescription = "Desc",
                QuantityInStock = 10,
                ProductImages = new List<IFormFile>() // Để trống ảnh để test logic DB trước
            };

            // Act
            var result = _controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Kiểm tra DB xem có sản phẩm chưa
            var productInDb = _context.Products.FirstOrDefault(p => p.ProductName == "Admin Product");
            Assert.NotNull(productInDb);
            Assert.Equal(50000, productInDb.ProductPrice);
        }

        // TC-04: Sửa sản phẩm thành công
        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesProduct()
        {
            // Arrange
            // Để chắc chắn, ta detach (ngắt theo dõi) các entity cũ để tránh lỗi "Tracking" khi update
            _context.ChangeTracker.Clear();

            var model = new EditProductViewModel
            {
                ProductId = 1, // ID này phải có trong DbMock
                ProductName = "Updated Name",
                ProductPrice = 99999,
                IdCategory = 1,
                QuantityInStock = 5,
                IsOnSale = true
            };

            // Act
            var result = await _controller.Edit(model, null, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);

            // Kiểm tra DB
            var updatedProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == 1);
            Assert.Equal("Updated Name", updatedProduct.ProductName);
            Assert.Equal(99999, updatedProduct.ProductPrice);
        }

        // TC-05: Xóa sản phẩm (Soft Delete)
        [Fact]
        public void Delete_Post_ValidId_SoftDeletes()
        {
            // Act (Xóa ID 1)
            var result = _controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Kiểm tra IsOnSale = false
            var deletedProduct = _context.Products.FirstOrDefault(p => p.ProductId == 1);
            Assert.NotNull(deletedProduct);
            Assert.False(deletedProduct.IsOnSale);
        }

        // TC-06: Lọc sản phẩm trong Admin
        [Fact]
        public void SearchProduct_ByName_ReturnsResults()
        {
            // Act
            var result = _controller.SearchProduct("findByProductName", "Bếp");

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsType<ProductSearchViewModel>(partialViewResult.Model);
            Assert.Contains(model.Products, p => p.ProductName.Contains("Bếp"));
        }

        // Test Statistics (Biểu đồ)
        [Fact]
        public async Task Statistics_ReturnsViewModelWithData()
        {
            // Act
            var result = await _controller.Statistics();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductStatisticsViewModel>(viewResult.Model);

            Assert.True(model.TotalProducts > 0);
            Assert.NotNull(model.TopSellingProducts);
            Assert.NotNull(model.LowStockProducts);
        }
    }
}