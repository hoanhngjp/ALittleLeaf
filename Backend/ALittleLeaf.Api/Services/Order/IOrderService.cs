using ALittleLeaf.Api.DTOs.Order;

namespace ALittleLeaf.Api.Services.Order
{
    public interface IOrderService
    {
        /// <summary>
        /// Validates the VNPay callback data, applies idempotency check, and marks the order as
        /// paid if not already done. Safe to call from both IPN and Return-URL endpoints.
        /// </summary>
        Task<PaymentResultDto> ConfirmPaymentAsync(IQueryCollection vnpayData);

        // ── Address ───────────────────────────────────────────────────────────
        Task<List<AddressDto>> GetUserAddressesAsync(long userId);
        Task<AddressDto?> GetAddressDetailAsync(int addressId);
        Task<AddressDto> AddAddressAsync(long userId, CreateAddressDto dto);
        Task<AddressDto?> UpdateAddressAsync(long userId, int addressId, CreateAddressDto dto);
        Task<bool> DeleteAddressAsync(long userId, int addressId);

        // ── Order ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Creates a Bill from the user's DB cart. Accepts an optional address id
        /// or inline address fields from <paramref name="dto"/>.
        /// For COD orders the cart is cleared and stock is decremented immediately.
        /// For VNPay orders the bill is saved with status "pending_vnpay";
        /// stock decrement happens after payment confirmation.
        /// </summary>
        Task<OrderDto> CreateOrderAsync(long userId, CreateOrderDto dto);

        /// <summary>Decrements stock. Called after VNPay payment confirmation.</summary>
        Task FulfillOrderAsync(int billId);

        Task<List<OrderDto>> GetOrderHistoryAsync(long userId);
        Task<OrderDetailDto?> GetOrderDetailAsync(int billId, long userId);
    }
}
