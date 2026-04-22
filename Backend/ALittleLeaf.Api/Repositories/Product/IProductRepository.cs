using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Product
{
    public interface IProductRepository
    {
        /// <summary>Returns a single product with its images and category, or null.</summary>
        Task<Models.Product?> GetByIdAsync(int productId);

        /// <summary>
        /// Applies category / keyword / price filters, returns the total count and the
        /// requested page of products (with primary image only).
        /// </summary>
        Task<(int TotalItems, IEnumerable<Models.Product> Items)> GetPaginatedAsync(
            int? categoryId,
            string? keyword,
            int? minPrice,
            int? maxPrice,
            int  page,
            int  pageSize);

        /// <summary>Returns up to <paramref name="count"/> products from the same category.</summary>
        Task<IEnumerable<Models.Product>> GetRelatedAsync(int productId, int categoryId, int count = 4);

        /// <summary>Live-search: up to 5 results whose name contains <paramref name="keyword"/>.</summary>
        Task<IEnumerable<Models.Product>> SearchLiveAsync(string keyword, int take = 5);
    }
}
