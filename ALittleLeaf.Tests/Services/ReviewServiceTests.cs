using ALittleLeaf.Api.DTOs.Review;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Repositories.Review;
using ALittleLeaf.Api.Services.Review;
using ALittleLeaf.Tests.Helpers;
using Moq;

namespace ALittleLeaf.Tests.Services
{
    public class ReviewServiceTests : IDisposable
    {
        private readonly ALittleLeaf.Api.Data.AlittleLeafDecorContext _context;
        private readonly ReviewService _service;
        private readonly Mock<IOrderRepository> _mockOrderRepo;

        private const long UserId    = 10;
        private const int  ProductId = 1;

        public ReviewServiceTests()
        {
            _context = DbContextFactory.Create();

            var reviewRepo     = new ReviewRepository(_context);
            _mockOrderRepo     = new Mock<IOrderRepository>();
            _service           = new ReviewService(reviewRepo, _mockOrderRepo.Object);
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        // ── Helpers ───────────────────────────────────────────────────────────

        private void AllowPurchase(bool allowed = true)
            => _mockOrderRepo
                .Setup(r => r.HasUserPurchasedProductAsync(UserId, ProductId))
                .ReturnsAsync(allowed);

        private async Task SeedUser()
        {
            if (!_context.Users.Any(u => u.UserId == UserId))
            {
                _context.Users.Add(new User
                {
                    UserId       = UserId,
                    UserEmail    = "reviewer@test.com",
                    UserFullname = "Nguyen Test",
                    UserRole     = "customer",
                    UserIsActive = true,
                    UserSex      = true,
                    UserBirthday = new DateOnly(1995, 1, 1),
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
        }

        // ── Tests ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateReview_HappyPath_ReturnsDto()
        {
            await SeedUser();
            AllowPurchase(true);

            var dto = new CreateReviewDto { ProductId = ProductId, Rating = 4, Comment = "Tốt lắm!" };
            var result = await _service.CreateReviewAsync(UserId, dto);

            Assert.NotNull(result);
            Assert.Equal(4, result.Rating);
            Assert.Equal("Tốt lắm!", result.Comment);
            Assert.Equal("Nguyen", result.FirstName);
        }

        [Fact]
        public async Task CreateReview_UserHasNoCompletedOrder_ThrowsInvalidOperationException()
        {
            AllowPurchase(false);

            var dto = new CreateReviewDto { ProductId = ProductId, Rating = 5 };
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateReviewAsync(UserId, dto));
        }

        [Fact]
        public async Task CreateReview_DuplicateReview_ThrowsInvalidOperationException()
        {
            await SeedUser();
            AllowPurchase(true);

            // First review succeeds
            await _service.CreateReviewAsync(UserId, new CreateReviewDto { ProductId = ProductId, Rating = 3 });

            // Second review for same product should throw
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateReviewAsync(UserId, new CreateReviewDto { ProductId = ProductId, Rating = 5 }));
        }

        [Fact]
        public async Task CreateReview_RatingOutOfRange_ThrowsArgumentException()
        {
            AllowPurchase(true);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateReviewAsync(UserId, new CreateReviewDto { ProductId = ProductId, Rating = 0 }));

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateReviewAsync(UserId, new CreateReviewDto { ProductId = ProductId, Rating = 6 }));
        }

        [Fact]
        public async Task GetProductRating_NoReviews_ReturnsNullAverageAndZeroCount()
        {
            var result = await _service.GetProductRatingAsync(ProductId);

            Assert.Equal(ProductId, result.ProductId);
            Assert.Null(result.AverageRating);
            Assert.Equal(0, result.ReviewCount);
        }

        [Fact]
        public async Task GetProductRating_WithReviews_ReturnsCorrectAverage()
        {
            await SeedUser();
            AllowPurchase(true);

            await _service.CreateReviewAsync(UserId, new CreateReviewDto { ProductId = ProductId, Rating = 4 });

            // Seed a second user's review directly
            _context.Users.Add(new User
            {
                UserId = 99, UserEmail = "u2@test.com", UserFullname = "Le Van B",
                UserRole = "customer", UserIsActive = true, UserSex = true,
                UserBirthday = new DateOnly(1998, 1, 1),
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            });
            _context.Reviews.Add(new Review
            {
                UserId = 99, ProductId = ProductId, Rating = 2, CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetProductRatingAsync(ProductId);

            Assert.Equal(2, result.ReviewCount);
            Assert.Equal(3.0, result.AverageRating); // (4+2)/2 = 3.0
        }
    }
}
