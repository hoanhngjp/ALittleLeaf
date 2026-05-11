using ALittleLeaf.Api.DTOs.Review;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Repositories.Review;

namespace ALittleLeaf.Api.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IOrderRepository _orderRepo;

        public ReviewService(IReviewRepository reviewRepo, IOrderRepository orderRepo)
        {
            _reviewRepo = reviewRepo;
            _orderRepo  = orderRepo;
        }

        public async Task<PagedReviewResultDto> GetProductReviewsAsync(int productId, int page, int pageSize)
        {
            var items = await _reviewRepo.GetByProductIdAsync(productId, page, pageSize);
            var total = await _reviewRepo.GetCountByProductIdAsync(productId);

            return new PagedReviewResultDto
            {
                Items     = items.Select(ToDto).ToList(),
                TotalCount = total,
                Page      = page,
                PageSize  = pageSize
            };
        }

        public async Task<ProductRatingDto> GetProductRatingAsync(int productId)
        {
            var average = await _reviewRepo.GetAverageRatingAsync(productId);
            var count   = await _reviewRepo.GetCountByProductIdAsync(productId);

            return new ProductRatingDto
            {
                ProductId     = productId,
                AverageRating = average.HasValue ? Math.Round(average.Value, 1) : null,
                ReviewCount   = count
            };
        }

        public async Task<ReviewDto?> GetMyReviewAsync(long userId, int productId)
        {
            var review = await _reviewRepo.GetByUserAndProductAsync(userId, productId);
            return review is null ? null : ToDto(review);
        }

        public async Task<ReviewDto> CreateReviewAsync(long userId, CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            if (dto.Comment?.Length > 500)
                throw new ArgumentException("Comment must not exceed 500 characters.");

            var hasPurchased = await _orderRepo.HasUserPurchasedProductAsync(userId, dto.ProductId);
            if (!hasPurchased)
                throw new InvalidOperationException("Bạn cần có đơn hàng hoàn thành chứa sản phẩm này để viết đánh giá.");

            var existing = await _reviewRepo.GetByUserAndProductAsync(userId, dto.ProductId);
            if (existing is not null)
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi.");

            var review = new Models.Review
            {
                UserId    = userId,
                ProductId = dto.ProductId,
                Rating    = dto.Rating,
                Comment   = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _reviewRepo.Add(review);
            await _reviewRepo.SaveChangesAsync();

            // Reload with user navigation for firstName
            var created = await _reviewRepo.GetByIdAsync(review.ReviewId);
            return ToDto(created!);
        }

        public async Task<PagedAdminReviewResultDto> GetAllReviewsAsync(int? productId, int page, int pageSize)
        {
            var items = await _reviewRepo.GetAllAsync(productId, page, pageSize);
            var total = await _reviewRepo.GetAllCountAsync(productId);

            return new PagedAdminReviewResultDto
            {
                Items     = items.Select(ToAdminDto).ToList(),
                TotalCount = total,
                Page      = page,
                PageSize  = pageSize
            };
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId);
            if (review is null) return false;

            _reviewRepo.Remove(review);
            await _reviewRepo.SaveChangesAsync();
            return true;
        }

        private static AdminReviewDto ToAdminDto(Models.Review r) => new()
        {
            ReviewId    = r.ReviewId,
            UserId      = r.UserId,
            UserName    = r.UserNavigation?.UserFullname ?? "—",
            UserEmail   = r.UserNavigation?.UserEmail   ?? "—",
            ProductId   = r.ProductId,
            ProductName = r.ProductNavigation?.ProductName ?? "—",
            Rating      = r.Rating,
            Comment     = r.Comment,
            CreatedAt   = r.CreatedAt
        };

        private static ReviewDto ToDto(Models.Review r) => new()
        {
            ReviewId  = r.ReviewId,
            UserId    = r.UserId,
            FirstName = r.UserNavigation?.UserFullname?.Split(' ').FirstOrDefault() ?? "Ẩn danh",
            Rating    = r.Rating,
            Comment   = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}
