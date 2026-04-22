using ALittleLeaf.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public CategoryRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Category>> GetAllWithSubCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.CategoryParentId == null || c.CategoryParentId == 0)   // roots: parent is NULL or 0
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Models.Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == name.ToLower());
        }

        public async Task<Models.Category?> GetByIdAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }
    }
}
