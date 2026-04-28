using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Order
{
    public interface IOrderRepository
    {
        // ── Address ───────────────────────────────────────────────────────────
        Task<List<AddressList>> GetAddressesByUserIdAsync(long userId);
        Task<AddressList?> GetAddressByIdAsync(int addressId);
        Task<AddressList> AddAddressAsync(AddressList address);
        Task UpdateAddressAsync(AddressList address);
        Task DeleteAddressAsync(AddressList address);

        // ── Bill ──────────────────────────────────────────────────────────────
        Task<Bill> CreateBillAsync(Bill bill);
        Task<Bill?> GetBillByIdAsync(int billId);
        Task<Bill?> GetBillByGhnOrderCodeAsync(string ghnOrderCode);
        Task<Bill?> GetBillByIdForUserAsync(int billId, long userId);
        Task<List<Bill>> GetBillsByUserIdAsync(long userId);
        Task AddBillDetailsAsync(IEnumerable<BillDetail> details);

        // ── Product (stock deduction) ─────────────────────────────────────────
        Task<Models.Product?> GetProductByIdAsync(int productId);

        // ── Background job ────────────────────────────────────────────────────
        /// <summary>Returns VNPay bills that are still PENDING and older than <paramref name="cutoffTime"/>.</summary>
        Task<List<Bill>> GetExpiredPendingVnpayOrdersAsync(DateTime cutoffTime);

        Task SaveChangesAsync();
    }
}
