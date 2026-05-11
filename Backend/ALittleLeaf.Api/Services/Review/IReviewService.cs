using ALittleLeaf.Api.DTOs.Review;

namespace ALittleLeaf.Api.Services.Review
{
    public interface IReviewService
    {
        Task<PagedReviewResultDto> GetProductReviewsAsync(int productId, int page, int pageSize);
        Task<ProductRatingDto> GetProductRatingAsync(int productId);
        Task<ReviewDto?> GetMyReviewAsync(long userId, int productId);
        Task<ReviewDto> CreateReviewAsync(long userId, CreateReviewDto dto);
        Task<PagedAdminReviewResultDto> GetAllReviewsAsync(int? productId, int page, int pageSize);
        Task<bool> DeleteReviewAsync(int reviewId);
    }
}
