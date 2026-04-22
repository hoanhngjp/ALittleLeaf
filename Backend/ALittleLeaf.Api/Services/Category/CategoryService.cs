using ALittleLeaf.Api.DTOs.Product;
using ALittleLeaf.Api.Repositories.Category;
using ALittleLeaf.Api.Repositories.Product;

namespace ALittleLeaf.Api.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IProductRepository  _productRepo;

        public CategoryService(
            ICategoryRepository categoryRepo,
            IProductRepository  productRepo)
        {
            _categoryRepo = categoryRepo;
            _productRepo  = productRepo;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var roots = await _categoryRepo.GetAllWithSubCategoriesAsync();

            return roots.Select(c => new CategoryDto
            {
                CategoryId       = c.CategoryId,
                CategoryName     = c.CategoryName,
                CategoryParentId = c.CategoryParentId,
                CategoryImg      = c.CategoryImg,
                SubCategories    = c.SubCategories.Select(s => new CategoryDto
                {
                    CategoryId       = s.CategoryId,
                    CategoryName     = s.CategoryName,
                    CategoryParentId = s.CategoryParentId,
                    CategoryImg      = s.CategoryImg
                })
            });
        }

        public async Task<PaginatedResultDto<ProductDto>?> GetProductsByCategoryNameAsync(
            string categoryName,
            int?   minPrice,
            int?   maxPrice,
            int    page,
            int    pageSize)
        {
            var category = await _categoryRepo.GetByNameAsync(categoryName);
            if (category == null) return null;

            var (totalItems, items) = await _productRepo.GetPaginatedAsync(
                category.CategoryId, null, minPrice, maxPrice, page, pageSize);

            return new PaginatedResultDto<ProductDto>
            {
                TotalItems = totalItems,
                Page       = page,
                PageSize   = pageSize,
                Items      = items.Select(p => new ProductDto
                {
                    ProductId       = p.ProductId,
                    IdCategory      = p.IdCategory,
                    ProductName     = p.ProductName,
                    ProductPrice    = p.ProductPrice,
                    QuantityInStock = p.QuantityInStock,
                    PrimaryImage    = p.ProductImages.FirstOrDefault(i => i.IsPrimary)?.ImgName
                })
            };
        }
    }
}
