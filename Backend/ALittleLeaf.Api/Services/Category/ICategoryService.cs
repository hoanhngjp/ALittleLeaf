using ALittleLeaf.Api.DTOs.Product;

namespace ALittleLeaf.Api.Services.Category
{
    public interface ICategoryService
    {
        /// <summary>Returns all root categories with their sub-categories.</summary>
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        /// <summary>
        /// Returns a paginated product list for the category identified by <paramref name="categoryName"/>.
        /// Returns null when the category is not found.
        /// </summary>
        Task<PaginatedResultDto<ProductDto>?> GetProductsByCategoryNameAsync(
            string categoryName,
            int?   minPrice,
            int?   maxPrice,
            int    page,
            int    pageSize);
    }
}
