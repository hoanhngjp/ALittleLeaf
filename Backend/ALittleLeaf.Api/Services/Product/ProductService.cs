using ALittleLeaf.Api.DTOs.Product;
using ALittleLeaf.Api.Repositories.Product;

namespace ALittleLeaf.Api.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(int productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return null;

            return new ProductDetailDto
            {
                ProductId          = product.ProductId,
                IdCategory         = product.IdCategory,
                CategoryName       = product.IdCategoryNavigation?.CategoryName ?? string.Empty,
                ProductName        = product.ProductName,
                ProductPrice       = product.ProductPrice,
                ProductDescription = product.ProductDescription,
                QuantityInStock    = product.QuantityInStock,
                IsOnSale           = product.IsOnSale,
                PrimaryImage       = product.ProductImages.FirstOrDefault(i => i.IsPrimary)?.ImgName,
                Images             = product.ProductImages.Select(i => i.ImgName).ToArray()
            };
        }

        public async Task<PaginatedResultDto<ProductDto>> GetPaginatedProductsAsync(
            int?    categoryId,
            string? keyword,
            int?    minPrice,
            int?    maxPrice,
            int     page,
            int     pageSize)
        {
            var (totalItems, items) = await _productRepo.GetPaginatedAsync(
                categoryId, keyword, minPrice, maxPrice, page, pageSize);

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

        public async Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return [];

            var related = await _productRepo.GetRelatedAsync(productId, product.IdCategory, count);

            return related.Select(p => new ProductDto
            {
                ProductId       = p.ProductId,
                IdCategory      = p.IdCategory,
                ProductName     = p.ProductName,
                ProductPrice    = p.ProductPrice,
                QuantityInStock = p.QuantityInStock,
                PrimaryImage    = p.ProductImages.FirstOrDefault(i => i.IsPrimary)?.ImgName
            });
        }

        public async Task<IEnumerable<ProductDto>> SearchLiveAsync(string keyword)
        {
            var results = await _productRepo.SearchLiveAsync(keyword);

            return results.Select(p => new ProductDto
            {
                ProductId       = p.ProductId,
                IdCategory      = p.IdCategory,
                ProductName     = p.ProductName,
                ProductPrice    = p.ProductPrice,
                QuantityInStock = p.QuantityInStock,
                PrimaryImage    = p.ProductImages.FirstOrDefault(i => i.IsPrimary)?.ImgName
            });
        }
    }
}
