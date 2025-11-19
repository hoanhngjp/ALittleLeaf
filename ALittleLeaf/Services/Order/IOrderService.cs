using ALittleLeaf.Models;
using ALittleLeaf.ViewModels;

namespace ALittleLeaf.Services.Order
{
    public interface IOrderService
    {
        Task<List<AddressList>> GetUserAddressesAsync(long userId);
        Task<AddressList> GetAddressDetailsAsync(int addressId);
        Task<Bill> CreateOrderFromSessionAsync(string paymentMethod, string paymentStatus);
        Task FulfillOrderAsync(int billId);
        Task<List<Bill>> GetOrderHistoryAsync(long userId);
        Task<Bill> GetBillByIdAsync(int billId, long userId);
        Task<List<OrderDetailViewModel>> GetBillDetailsAsync(int billId);
    }
}
