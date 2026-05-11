using ALittleLeaf.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Review
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public ReviewRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public Task<List<Models.Review>> GetAllAsync(int? productId, int page, int pageSize)
        {
            var q = _context.Reviews
                            .Include(r => r.UserNavigation)
                            .Include(r => r.ProductNavigation)
                            .AsQueryable();
            if (productId.HasValue)
                q = q.Where(r => r.ProductId == productId.Value);
            return q.OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }

        public Task<int> GetAllCountAsync(int? productId)
        {
            var q = _context.Reviews.AsQueryable();
            if (productId.HasValue)
                q = q.Where(r => r.ProductId == productId.Value);
            return q.CountAsync();
        }

        public Task<List<Models.Review>> GetByProductIdAsync(int productId, int page, int pageSize)
            => _context.Reviews
                       .Include(r => r.UserNavigation)
                       .Where(r => r.ProductId == productId)
                       .OrderByDescending(r => r.CreatedAt)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();

        public Task<int> GetCountByProductIdAsync(int productId)
            => _context.Reviews.CountAsync(r => r.ProductId == productId);

        public Task<double?> GetAverageRatingAsync(int productId)
            => _context.Reviews
                       .Where(r => r.ProductId == productId)
                       .Select(r => (double?)r.Rating)
                       .AverageAsync();

        public Task<Models.Review?> GetByUserAndProductAsync(long userId, int productId)
            => _context.Reviews
                       .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);

        public Task<Models.Review?> GetByIdAsync(int reviewId)
            => _context.Reviews.FindAsync(reviewId).AsTask();

        public void Add(Models.Review review) => _context.Reviews.Add(review);

        public void Remove(Models.Review review) => _context.Reviews.Remove(review);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
