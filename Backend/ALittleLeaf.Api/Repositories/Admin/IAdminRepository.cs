using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Admin
{
    public interface IAdminRepository
    {
        // ── Products ──────────────────────────────────────────────────────────
        Task<(int Total, IEnumerable<Models.Product> Items)> GetProductsPagedAsync(
            string? keyword, int? categoryId, bool? isOnSale,
            string? sortBy, bool isDescending,
            int page, int pageSize);

        Task<Models.Product?> GetProductByIdAsync(int productId);
        Task<Models.Product> CreateProductAsync(Models.Product product);
        Task UpdateProductAsync(Models.Product product);
        Task DeleteProductAsync(Models.Product product);

        Task<ProductImage?> GetProductImageByIdAsync(int imgId);
        Task AddProductImageAsync(ProductImage image);
        Task DeleteProductImageAsync(ProductImage image);

        // ── Orders ────────────────────────────────────────────────────────────
        Task<(int Total, IEnumerable<Bill> Items)> GetOrdersPagedAsync(
            string? keyword, string? orderStatus, string? shippingStatus, string? paymentStatus,
            DateOnly? startDate, DateOnly? endDate,
            string? sortBy, bool isDescending,
            int page, int pageSize);

        Task<Bill?> GetOrderByIdAsync(int billId);
        Task UpdateOrderAsync(Bill bill);

        // ── Users ─────────────────────────────────────────────────────────────
        Task<(int Total, IEnumerable<User> Items)> GetUsersPagedAsync(
            string? keyword, bool? isActive, string? userRole, bool? userSex,
            string? sortBy, bool isDescending,
            int page, int pageSize);

        Task<User?> GetUserByIdAsync(long userId);
        Task<bool>  UserEmailExistsAsync(string email);
        Task<User>  CreateUserAsync(User user);
        Task UpdateUserAsync(User user);

        // ── Dashboard ─────────────────────────────────────────────────────────
        Task<long>  GetTotalRevenueAsync(DateOnly? startDate = null, DateOnly? endDate = null);
        Task<int>   GetTotalOrderCountAsync(DateOnly? startDate = null, DateOnly? endDate = null);
        Task<int>   GetPendingOrderCountAsync(DateOnly? startDate = null, DateOnly? endDate = null);
        Task<int>   GetTotalUserCountAsync();
        Task<int>   GetTotalProductCountAsync();

        Task<IEnumerable<Models.Product>> GetLowStockProductsAsync(int threshold = 5, int take = 10);
        Task<IEnumerable<(int ProductId, string ProductName, int TotalSold, long TotalRevenue, string? PrimaryImage)>>
            GetTopSellingProductsAsync(int take = 5);
        Task<IEnumerable<(int Year, int Month, long Revenue)>> GetRevenueByMonthAsync(DateOnly? startDate = null, DateOnly? endDate = null);
        Task<IEnumerable<(string CategoryName, long Revenue)>> GetRevenueByCategoryAsync(DateOnly? startDate = null, DateOnly? endDate = null);

        Task SaveChangesAsync();
    }
}
