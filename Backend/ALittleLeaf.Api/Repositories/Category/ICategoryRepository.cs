using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Category
{
    public interface ICategoryRepository
    {
        /// <summary>Returns all root categories with their sub-categories eagerly loaded.</summary>
        Task<IEnumerable<Models.Category>> GetAllWithSubCategoriesAsync();

        /// <summary>Finds a category by its exact name (case-insensitive).</summary>
        Task<Models.Category?> GetByNameAsync(string name);

        /// <summary>Finds a category by its primary key.</summary>
        Task<Models.Category?> GetByIdAsync(int categoryId);
    }
}
