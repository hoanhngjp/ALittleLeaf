using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Product; // Namespace của bạn
using ALittleLeaf.Tests.Helpers;    // DbContextFactory
using ALittleLeaf.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _context = DbContextFactory.Create();
            _service = new ProductService(_context);
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        #region CATALOG TESTS (TC-01 -> TC-06)

        [Fact] // TC-01 & TC-02: Tìm kiếm
        public async Task GetPaginatedProductsAsync_SearchKeyword_ReturnsCorrectProducts()
        {
            // Act 1: Tìm "Bếp" (Có kết quả - xem MockData)
            var resultFound = await _service.GetPaginatedProductsAsync(null, "Bếp", null, null, 1, 10);

            // Act 2: Tìm "Mũ" (Không có kết quả)
            var resultNotFound = await _service.GetPaginatedProductsAsync(null, "Mũ", null, null, 1, 10);

            // Assert
            Assert.Contains(resultFound.Products, p => p.ProductName.Contains("Bếp") || p.ProductName.Contains("Ấm"));
            Assert.Empty(resultNotFound.Products);
            Assert.Equal("Bếp", resultFound.PageTitle);
        }

        [Fact] // TC-03: Lọc danh mục
        public async Task GetPaginatedProductsAsync_FilterCategory_ReturnsProductsInCategory()
        {
            // Act: Lọc danh mục ID 2 (Cây cảnh - xem MockData)
            var result = await _service.GetPaginatedProductsAsync(2, null, null, null, 1, 10);

            // Assert
            Assert.NotEmpty(result.Products);
            Assert.All(result.Products, p => Assert.Equal(2, p.IdCategory));
            Assert.Equal("Cây cảnh", result.PageTitle);
        }

        [Fact] // TC-04: Lọc giá
        public async Task GetPaginatedProductsAsync_FilterPrice_ReturnsProductsInRange()
        {
            // Act: Giá > 2.000.000 (Chỉ có Bếp Từ Cao Cấp)
            var result = await _service.GetPaginatedProductsAsync(null, null, 2000000, null, 1, 10);

            // Assert
            Assert.Single(result.Products);
            Assert.Equal("Bếp Từ Cao Cấp", result.Products.First().ProductName);
        }

        [Fact] // TC-06: Phân trang
        public async Task GetPaginatedProductsAsync_Pagination_ReturnsCorrectMetaData()
        {
            // Arrange: Thêm nhiều sản phẩm giả
            for (int i = 0; i < 20; i++)
            {
                _context.Products.Add(new Product
                {
                    ProductName = $"Prod {i}",
                    ProductPrice = 100,
                    IdCategory = 1,
                    QuantityInStock = 10,
                    IsOnSale = true
                });
            }
            await _context.SaveChangesAsync();

            // Act: Lấy trang 2, pageSize = 5
            var result = await _service.GetPaginatedProductsAsync(null, null, null, null, 2, 5);

            // Assert
            Assert.Equal(2, result.Pagination.CurrentPage);
            Assert.Equal(5, result.Products.Count);
            Assert.True(result.Pagination.TotalItems >= 20);
        }

        #endregion

        #region DETAIL TESTS (TC-07)

        [Fact]
        public async Task GetProductDetailAsync_ValidId_ReturnsViewModel()
        {
            // Act
            var result = await _service.GetProductDetailAsync(1); // ID 1 = Ấm Tráng Men

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProductId);
            Assert.NotNull(result.CategoryName); // Kiểm tra Include Category hoạt động
        }

        #endregion
    }
}