using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Product;
using ALittleLeaf.Api.Services.Product;
using ALittleLeaf.Tests.Helpers;

namespace ALittleLeaf.Tests.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly ProductService          _service;

        public ProductServiceTests()
        {
            _context = DbContextFactory.Create();
            var repo = new ProductRepository(_context);
            _service = new ProductService(repo);
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        // ── Catalog ───────────────────────────────────────────────────────────

        // TC-01/02: Tìm kiếm theo keyword
        [Fact]
        public async Task GetPaginated_SearchKeyword_ReturnsMatchingProducts()
        {
            // "Bếp" should match "Bếp Từ Cao Cấp" and "Ấm Tráng Men" is in category "Bếp"
            // but keyword search matches ProductName only — "Bếp" matches product 2
            var found    = await _service.GetPaginatedProductsAsync(null, "Bếp", null, null, 1, 10);
            var notFound = await _service.GetPaginatedProductsAsync(null, "Mũ",  null, null, 1, 10);

            Assert.True(found.TotalItems >= 1);
            Assert.All(found.Items, p => Assert.Contains("Bếp", p.ProductName));
            Assert.Equal(0, notFound.TotalItems);
        }

        // TC-03: Lọc theo danh mục
        [Fact]
        public async Task GetPaginated_FilterCategory_ReturnsOnlyThatCategory()
        {
            var result = await _service.GetPaginatedProductsAsync(2, null, null, null, 1, 10);

            Assert.NotEmpty(result.Items);
            Assert.All(result.Items, p => Assert.Equal(2, p.IdCategory));
        }

        // TC-04: Lọc theo giá tối thiểu
        [Fact]
        public async Task GetPaginated_MinPrice_FiltersCorrectly()
        {
            var result = await _service.GetPaginatedProductsAsync(null, null, 2000000, null, 1, 10);

            Assert.Single(result.Items);
            Assert.Equal("Bếp Từ Cao Cấp", result.Items.First().ProductName);
        }

        // TC-06: Phân trang
        [Fact]
        public async Task GetPaginated_PageTwo_ReturnsCorrectSlice()
        {
            for (int i = 0; i < 20; i++)
            {
                _context.Products.Add(new Product
                {
                    ProductName = $"Extra Prod {i}", ProductPrice = 100,
                    IdCategory = 1, QuantityInStock = 10, IsOnSale = true,
                    RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, (byte)(i / 256), (byte)(i % 256) }
                });
            }
            await _context.SaveChangesAsync();

            var result = await _service.GetPaginatedProductsAsync(null, null, null, null, 2, 5);

            Assert.Equal(2,  result.Page);
            Assert.Equal(5,  result.Items.Count());
            Assert.True(result.TotalItems >= 20);
        }

        // TC-07: Chi tiết sản phẩm
        [Fact]
        public async Task GetProductDetail_ValidId_ReturnsDto()
        {
            var result = await _service.GetProductDetailAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.ProductId);
            Assert.Equal("Ấm Tráng Men", result.ProductName);
            // CategoryName populated via Include
            Assert.NotNull(result.CategoryName);
        }

        [Fact]
        public async Task GetProductDetail_InvalidId_ReturnsNull()
        {
            var result = await _service.GetProductDetailAsync(9999);
            Assert.Null(result);
        }
    }
}
