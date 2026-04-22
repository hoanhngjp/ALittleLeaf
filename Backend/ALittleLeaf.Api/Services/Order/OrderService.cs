using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Api.Services.VNPay;

namespace ALittleLeaf.Api.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartService     _cartService;
        private readonly IVnPayService    _vnPayService;

        public OrderService(IOrderRepository orderRepo, ICartService cartService, IVnPayService vnPayService)
        {
            _orderRepo    = orderRepo;
            _cartService  = cartService;
            _vnPayService = vnPayService;
        }

        // ── Payment confirmation (idempotent) ─────────────────────────────────

        public async Task<PaymentResultDto> ConfirmPaymentAsync(IQueryCollection vnpayData)
        {
            var response = _vnPayService.PaymentExecute(vnpayData);

            if (response == null)
                return new PaymentResultDto { Message = "Invalid response from VNPay." };

            // vnp_TxnRef is "{billId}_{ticks}" — extract the billId prefix
            var txnRefParts = (response.OrderId ?? "").Split('_');
            if (!int.TryParse(txnRefParts[0], out int billId))
                return new PaymentResultDto { VnpResponseCode = "01", Message = "Invalid order reference." };

            var bill = await _orderRepo.GetBillByIdAsync(billId);
            if (bill == null)
                return new PaymentResultDto { VnpResponseCode = "01", Message = "Order not found.", BillId = billId };

            // Idempotency guard — IPN and Return URL may both fire; only process once
            if (bill.PaymentStatus == "paid")
                return new PaymentResultDto { Success = true, BillId = billId, AlreadyPaid = true,
                    VnpResponseCode = "00", Message = "Order already confirmed." };

            if (!response.Success || response.VnPayResponseCode != "00")
                return new PaymentResultDto { BillId = billId, VnpResponseCode = response.VnPayResponseCode,
                    Message = "Payment was not successful." };

            bill.PaymentStatus = "paid";
            bill.IsConfirmed   = true;
            bill.UpdatedAt     = DateTime.Now;
            await _orderRepo.SaveChangesAsync();

            await FulfillOrderAsync(billId);
            await _cartService.ClearCartAsync(bill.IdUser);

            return new PaymentResultDto { Success = true, BillId = billId,
                VnpResponseCode = "00", Message = "Payment confirmed." };
        }

        // ── Address ───────────────────────────────────────────────────────────

        public async Task<List<AddressDto>> GetUserAddressesAsync(long userId)
        {
            var addresses = await _orderRepo.GetAddressesByUserIdAsync(userId);
            return addresses.Select(MapAddressToDto).ToList();
        }

        public async Task<AddressDto?> GetAddressDetailAsync(int addressId)
        {
            var address = await _orderRepo.GetAddressByIdAsync(addressId);
            return address == null ? null : MapAddressToDto(address);
        }

        public async Task<AddressDto> AddAddressAsync(long userId, CreateAddressDto dto)
        {
            // If the new address is default, clear the flag on all existing addresses first
            if (dto.AdrsIsDefault)
            {
                var existing = await _orderRepo.GetAddressesByUserIdAsync(userId);
                foreach (var a in existing)
                {
                    a.AdrsIsDefault = false;
                    await _orderRepo.UpdateAddressAsync(a);
                }
                await _orderRepo.SaveChangesAsync();
            }

            var address = new AddressList
            {
                IdUser       = userId,
                AdrsFullname = dto.AdrsFullname,
                AdrsAddress  = dto.AdrsAddress,
                AdrsPhone    = dto.AdrsPhone,
                AdrsIsDefault = dto.AdrsIsDefault,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            };

            var saved = await _orderRepo.AddAddressAsync(address);
            return MapAddressToDto(saved);
        }

        public async Task<AddressDto?> UpdateAddressAsync(long userId, int addressId, CreateAddressDto dto)
        {
            var address = await _orderRepo.GetAddressByIdAsync(addressId);
            if (address == null || address.IdUser != userId) return null;

            if (dto.AdrsIsDefault && !address.AdrsIsDefault)
            {
                var all = await _orderRepo.GetAddressesByUserIdAsync(userId);
                foreach (var a in all.Where(a => a.AdrsId != addressId))
                {
                    a.AdrsIsDefault = false;
                    await _orderRepo.UpdateAddressAsync(a);
                }
            }

            address.AdrsFullname  = dto.AdrsFullname;
            address.AdrsAddress   = dto.AdrsAddress;
            address.AdrsPhone     = dto.AdrsPhone;
            address.AdrsIsDefault = dto.AdrsIsDefault;
            address.UpdatedAt     = DateTime.UtcNow;

            await _orderRepo.UpdateAddressAsync(address);
            await _orderRepo.SaveChangesAsync();
            return MapAddressToDto(address);
        }

        public async Task<bool> DeleteAddressAsync(long userId, int addressId)
        {
            var address = await _orderRepo.GetAddressByIdAsync(addressId);
            if (address == null || address.IdUser != userId) return false;

            await _orderRepo.DeleteAddressAsync(address);
            await _orderRepo.SaveChangesAsync();
            return true;
        }

        // ── Order ─────────────────────────────────────────────────────────────

        public async Task<OrderDto> CreateOrderAsync(long userId, CreateOrderDto dto)
        {
            // 1. Resolve / create shipping address
            int adrsId;
            if (dto.AddressId.HasValue)
            {
                var adrs = await _orderRepo.GetAddressByIdAsync(dto.AddressId.Value)
                           ?? throw new InvalidOperationException("Address not found.");
                if (adrs.IdUser != userId)
                    throw new InvalidOperationException("Address does not belong to this user.");
                adrsId = adrs.AdrsId;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.NewFullName) ||
                    string.IsNullOrWhiteSpace(dto.NewAddress)  ||
                    string.IsNullOrWhiteSpace(dto.NewPhone))
                    throw new InvalidOperationException("Address information is incomplete.");

                var newAdrs = new AddressList
                {
                    IdUser       = userId,
                    AdrsFullname = dto.NewFullName,
                    AdrsAddress  = dto.NewAddress,
                    AdrsPhone    = dto.NewPhone,
                    AdrsIsDefault = false,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow
                };
                var saved = await _orderRepo.AddAddressAsync(newAdrs);
                adrsId = saved.AdrsId;
            }

            // 2. Read cart from DB (Phase 5)
            var cart = await _cartService.GetCartAsync(userId);
            if (!cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            int totalAmount = cart.Items.Sum(i => i.LineTotal);

            string paymentStatus = dto.PaymentMethod.ToUpper() == "VNPAY"
                ? "pending_vnpay"
                : "pending";

            // 3. Create Bill inside a transaction via repository
            var bill = new Bill
            {
                IdUser        = userId,
                IdAdrs        = adrsId,
                DateCreated   = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount   = totalAmount,
                PaymentMethod = dto.PaymentMethod.ToUpper(),
                PaymentStatus = paymentStatus,
                IsConfirmed   = false,
                ShippingStatus = "not_fulfilled",
                Note          = dto.Note,
                UpdatedAt     = DateTime.Now
            };

            var createdBill = await _orderRepo.CreateBillAsync(bill);

            // 4. Create BillDetails from cart items
            var details = cart.Items.Select(i => new BillDetail
            {
                IdBill     = createdBill.BillId,
                IdProduct  = i.ProductId,
                Quantity   = i.Quantity,
                UnitPrice  = i.ProductPrice,
                TotalPrice = i.LineTotal,
                CreatedAt  = DateTime.UtcNow,
                UpdatedAt  = DateTime.UtcNow
            }).ToList();

            await _orderRepo.AddBillDetailsAsync(details);

            // 5. For COD: fulfill immediately (deduct stock + clear cart)
            if (dto.PaymentMethod.ToUpper() == "COD")
            {
                await FulfillOrderAsync(createdBill.BillId);
                await _cartService.ClearCartAsync(userId);
            }

            // 6. Return the fresh bill with address
            var result = await _orderRepo.GetBillByIdAsync(createdBill.BillId);
            return MapBillToDto(result!);
        }

        public async Task FulfillOrderAsync(int billId)
        {
            var bill = await _orderRepo.GetBillByIdAsync(billId)
                       ?? throw new InvalidOperationException("Order not found.");

            foreach (var item in bill.BillDetails)
            {
                var product = await _orderRepo.GetProductByIdAsync(item.IdProduct)
                              ?? throw new InvalidOperationException($"Product {item.IdProduct} not found.");

                if (product.QuantityInStock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Product '{product.ProductName}' is out of stock.");

                product.QuantityInStock -= item.Quantity;
            }

            // Mark the order as confirmed and paid when fulfilled (COD path)
            bill.IsConfirmed   = true;
            bill.PaymentStatus = "paid";
            bill.UpdatedAt     = DateTime.Now;

            await _orderRepo.SaveChangesAsync();
        }

        public async Task<List<OrderDto>> GetOrderHistoryAsync(long userId)
        {
            var bills = await _orderRepo.GetBillsByUserIdAsync(userId);
            return bills.Select(MapBillToDto).ToList();
        }

        public async Task<OrderDetailDto?> GetOrderDetailAsync(int billId, long userId)
        {
            var bill = await _orderRepo.GetBillByIdForUserAsync(billId, userId);
            if (bill == null) return null;

            var dto = new OrderDetailDto
            {
                BillId         = bill.BillId,
                DateCreated    = bill.DateCreated,
                TotalAmount    = bill.TotalAmount,
                PaymentMethod  = bill.PaymentMethod,
                PaymentStatus  = bill.PaymentStatus,
                IsConfirmed    = bill.IsConfirmed,
                ShippingStatus = bill.ShippingStatus,
                Note           = bill.Note,
                ShippingAddress = bill.IdAdrsNavigation == null
                    ? null
                    : MapAddressToDto(bill.IdAdrsNavigation),
                Items = bill.BillDetails.Select(bd => new OrderLineItemDto
                {
                    BillDetailId = bd.BillDetailId,
                    ProductId    = bd.IdProduct,
                    ProductName  = bd.IdProductNavigation?.ProductName ?? string.Empty,
                    ProductImg   = bd.IdProductNavigation?.ProductImages
                                       .FirstOrDefault(i => i.IsPrimary)?.ImgName,
                    Quantity   = bd.Quantity,
                    UnitPrice  = bd.UnitPrice,
                    TotalPrice = bd.TotalPrice
                }).ToList()
            };

            return dto;
        }

        // ── Mapping helpers ───────────────────────────────────────────────────

        private static AddressDto MapAddressToDto(AddressList a) => new()
        {
            AdrsId       = a.AdrsId,
            AdrsFullname = a.AdrsFullname,
            AdrsAddress  = a.AdrsAddress,
            AdrsPhone    = a.AdrsPhone,
            AdrsIsDefault = a.AdrsIsDefault
        };

        private static OrderDto MapBillToDto(Bill b) => new()
        {
            BillId         = b.BillId,
            DateCreated    = b.DateCreated,
            TotalAmount    = b.TotalAmount,
            PaymentMethod  = b.PaymentMethod,
            PaymentStatus  = b.PaymentStatus,
            IsConfirmed    = b.IsConfirmed,
            ShippingStatus = b.ShippingStatus,
            Note           = b.Note,
            ShippingAddress = b.IdAdrsNavigation == null
                ? null
                : MapAddressToDto(b.IdAdrsNavigation)
        };
    }
}
