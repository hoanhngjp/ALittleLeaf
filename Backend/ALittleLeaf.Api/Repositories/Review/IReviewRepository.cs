using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Review
{
    public interface IReviewRepository
    {
        Task<List<Models.Review>> GetByProductIdAsync(int productId, int page, int pageSize);
        Task<int> GetCountByProductIdAsync(int productId);
        Task<double?> GetAverageRatingAsync(int productId);
        Task<List<Models.Review>> GetAllAsync(int? productId, int page, int pageSize);
        Task<int> GetAllCountAsync(int? productId);
        Task<Models.Review?> GetByUserAndProductAsync(long userId, int productId);
        Task<Models.Review?> GetByIdAsync(int reviewId);
        void Add(Models.Review review);
        void Remove(Models.Review review);
        Task SaveChangesAsync();
    }
}
