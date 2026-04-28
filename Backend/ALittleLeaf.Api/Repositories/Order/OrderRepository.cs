using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public OrderRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        // ── Address ───────────────────────────────────────────────────────────

        public Task<List<AddressList>> GetAddressesByUserIdAsync(long userId)
            => _context.AddressLists
                       .Where(a => a.IdUser == userId)
                       .OrderByDescending(a => a.AdrsIsDefault)
                       .ThenByDescending(a => a.CreatedAt)
                       .ToListAsync();

        public Task<AddressList?> GetAddressByIdAsync(int addressId)
            => _context.AddressLists.FirstOrDefaultAsync(a => a.AdrsId == addressId);

        public async Task<AddressList> AddAddressAsync(AddressList address)
        {
            _context.AddressLists.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public Task UpdateAddressAsync(AddressList address)
        {
            _context.AddressLists.Update(address);
            return Task.CompletedTask;
        }

        public void DeleteAddress(AddressList address)
            => _context.AddressLists.Remove(address);

        Task IOrderRepository.DeleteAddressAsync(AddressList address)
        {
            _context.AddressLists.Remove(address);
            return Task.CompletedTask;
        }

        // ── Bill ──────────────────────────────────────────────────────────────

        public async Task<Bill> CreateBillAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public Task<Bill?> GetBillByIdAsync(int billId)
            => _context.Bills
                       .Include(b => b.IdAdrsNavigation)
                       .Include(b => b.BillDetails)
                       .FirstOrDefaultAsync(b => b.BillId == billId);

        public Task<Bill?> GetBillByGhnOrderCodeAsync(string ghnOrderCode)
            => _context.Bills
                       .FirstOrDefaultAsync(b => b.GhnOrderCode == ghnOrderCode);

        public Task<Bill?> GetBillByIdForUserAsync(int billId, long userId)
            => _context.Bills
                       .Include(b => b.IdAdrsNavigation)
                       .Include(b => b.BillDetails)
                           .ThenInclude(bd => bd.IdProductNavigation)
                               .ThenInclude(p => p.ProductImages)
                       .FirstOrDefaultAsync(b => b.BillId == billId && b.IdUser == userId);

        public Task<List<Bill>> GetBillsByUserIdAsync(long userId)
            => _context.Bills
                       .Include(b => b.IdAdrsNavigation)
                       .Where(b => b.IdUser == userId)
                       .OrderByDescending(b => b.DateCreated)
                       .ThenByDescending(b => b.BillId)
                       .ToListAsync();

        public async Task AddBillDetailsAsync(IEnumerable<BillDetail> details)
        {
            _context.BillDetails.AddRange(details);
            await _context.SaveChangesAsync();
        }

        // ── Product ───────────────────────────────────────────────────────────

        public Task<Models.Product?> GetProductByIdAsync(int productId)
            => _context.Products.FindAsync(productId).AsTask();

        // ── Background job ────────────────────────────────────────────────────

        public Task<List<Bill>> GetExpiredPendingVnpayOrdersAsync(DateTime cutoffTime)
            => _context.Bills
                       .Include(b => b.BillDetails)
                       .Where(b => b.PaymentMethod == "VNPAY"
                                && b.PaymentStatus == "pending_vnpay"
                                && b.OrderStatus   == "PENDING"
                                && b.CreatedAtTime < cutoffTime)
                       .ToListAsync();

        // ── Persistence ───────────────────────────────────────────────────────

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
