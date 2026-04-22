using ALittleLeaf.Api.DTOs.Product;

namespace ALittleLeaf.Api.Services.Product
{
    public interface IProductService
    {
        /// <summary>Returns full product detail, or null when not found.</summary>
        Task<ProductDetailDto?> GetProductDetailAsync(int productId);

        /// <summary>Returns a paginated, filtered list of on-sale products.</summary>
        Task<PaginatedResultDto<ProductDto>> GetPaginatedProductsAsync(
            int?    categoryId,
            string? keyword,
            int?    minPrice,
            int?    maxPrice,
            int     page,
            int     pageSize);

        /// <summary>Returns up to <paramref name="count"/> related products from the same category.</summary>
        Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count = 4);

        /// <summary>Live-search: returns up to 5 lightweight results.</summary>
        Task<IEnumerable<ProductDto>> SearchLiveAsync(string keyword);
    }
}
