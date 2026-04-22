using ALittleLeaf.Api.DTOs.Admin;

namespace ALittleLeaf.Api.Services.Admin
{
    public interface IAdminService
    {
        // ── Products ──────────────────────────────────────────────────────────
        Task<PaginatedAdminResultDto<AdminProductDto>> GetProductsAsync(
            string? keyword, int? categoryId, bool? isOnSale,
            string? sortBy, bool isDescending,
            int page, int pageSize);

        Task<AdminProductDto?> GetProductByIdAsync(int productId);
        Task<AdminProductDto>  CreateProductAsync(CreateProductDto dto);
        Task<AdminProductDto?> UpdateProductAsync(int productId, UpdateProductDto dto);
        Task<bool>             DeleteProductAsync(int productId);

        // ── Orders ────────────────────────────────────────────────────────────
        Task<PaginatedAdminResultDto<AdminOrderDto>> GetOrdersAsync(
            string? keyword, string? shippingStatus, string? paymentStatus,
            DateOnly? startDate, DateOnly? endDate,
            string? sortBy, bool isDescending,
            int page, int pageSize);

        Task<AdminOrderDetailDto?> GetOrderByIdAsync(int billId);
        Task<AdminOrderDto?>       UpdateOrderStatusAsync(int billId, UpdateOrderStatusDto dto);

        // ── Users ─────────────────────────────────────────────────────────────
        Task<PaginatedAdminResultDto<AdminUserDto>> GetUsersAsync(
            string? keyword, bool? isActive, string? userRole, bool? userSex,
            string? sortBy, bool isDescending,
            int page, int pageSize);

        Task<AdminUserDto?> GetUserByIdAsync(long userId);
        Task<(AdminUserDto? User, string? Error)> CreateUserAsync(AdminCreateUserDto dto);
        Task<AdminUserDto?> UpdateUserAsync(long userId, AdminUpdateUserDto dto);

        // ── Dashboard ─────────────────────────────────────────────────────────
        Task<DashboardDto> GetDashboardAsync(DateOnly? startDate = null, DateOnly? endDate = null);
    }
}
