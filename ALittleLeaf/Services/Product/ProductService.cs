using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;

using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly AlittleLeafDecorContext _context;

        public ProductService(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public async Task<ProductDetailViewModel> GetProductDetailAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.IdCategoryNavigation)
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetailViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductDescription = p.ProductDescription,
                    QuantityInStock = p.QuantityInStock,
                    IsOnSale = p.IsOnSale,
                    ProductImages = p.ProductImages.Select(img => img.ImgName).ToArray(),
                    PrimaryImage = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName,
                    CategoryName = p.IdCategoryNavigation.CategoryName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProductServiceResult> GetPaginatedProductsAsync(int? categoryId, string keyword, int page, int pageSize)
        {
            var productsQuery = _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.IsOnSale);

            string pageTitle = "Tất cả sản phẩm";

            // Logic lọc theo Danh mục
            if (categoryId.HasValue)
            {
                var category = await _context.Categories.FindAsync(categoryId.Value);
                if (category != null)
                {
                    pageTitle = category.CategoryName;
                    // Kiểm tra danh mục con
                    var hasSubCategories = await _context.Categories.AnyAsync(c => c.CategoryParentId == categoryId);
                    if (hasSubCategories)
                    {
                        var subCategoryIds = await _context.Categories
                            .Where(c => c.CategoryParentId == categoryId)
                            .Select(c => c.CategoryId)
                            .ToListAsync();
                        productsQuery = productsQuery.Where(p => subCategoryIds.Contains(p.IdCategory));
                    }
                    else
                    {
                        productsQuery = productsQuery.Where(p => p.IdCategory == categoryId);
                    }
                }
            }
            // Logic lọc theo Từ khóa
            else if (!string.IsNullOrEmpty(keyword))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(keyword));
                pageTitle = keyword; // Gán từ khóa vào title để ViewBag dùng
            }

            int totalItems = await productsQuery.CountAsync();

            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.QuantityInStock,
                    ProductImg = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName
                })
                .ToListAsync();

            // Trả về kết quả tổng hợp
            return new ProductServiceResult
            {
                Products = products,
                Pagination = new Paginate(totalItems, page, pageSize),
                PageTitle = pageTitle,
                CategoryId = categoryId
            };
        }

        public async Task<List<ProductViewModel>> GetLiveSearchResultsAsync(string keyword)
        {
            return await _context.Products
                .Where(p => p.ProductName.Contains(keyword) && p.IsOnSale)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductImg = p.ProductImages.FirstOrDefault(i => i.IsPrimary).ImgName,
                    ProductQuantity = p.QuantityInStock
                })
                .Take(5)
                .ToListAsync();
        }
    }
}
