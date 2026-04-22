using ALittleLeaf.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public ProductRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public async Task<Models.Product?> GetByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.IdCategoryNavigation)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<(int TotalItems, IEnumerable<Models.Product> Items)> GetPaginatedAsync(
            int? categoryId,
            string? keyword,
            int? minPrice,
            int? maxPrice,
            int  page,
            int  pageSize)
        {
            var query = _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.IsOnSale)
                .AsQueryable();

            // ── Category filter (supports parent → child expansion) ────────────
            if (categoryId.HasValue)
            {
                var hasChildren = await _context.Categories
                    .AnyAsync(c => c.CategoryParentId == categoryId);

                if (hasChildren)
                {
                    var childIds = await _context.Categories
                        .Where(c => c.CategoryParentId == categoryId)
                        .Select(c => c.CategoryId)
                        .ToListAsync();

                    query = query.Where(p => childIds.Contains(p.IdCategory));
                }
                else
                {
                    query = query.Where(p => p.IdCategory == categoryId);
                }
            }
            // ── Keyword filter (mutually exclusive with category in legacy) ────
            else if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword));
            }

            // ── Price filters ─────────────────────────────────────────────────
            if (minPrice.HasValue)
                query = query.Where(p => p.ProductPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.ProductPrice <= maxPrice.Value);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, items);
        }

        public async Task<IEnumerable<Models.Product>> GetRelatedAsync(
            int productId, int categoryId, int count = 4)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.IdCategory == categoryId && p.ProductId != productId && p.IsOnSale)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Product>> SearchLiveAsync(string keyword, int take = 5)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.ProductName.Contains(keyword) && p.IsOnSale)
                .Take(take)
                .ToListAsync();
        }
    }
}
