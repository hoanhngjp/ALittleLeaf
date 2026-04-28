using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Api.Services.Shipping;
using ALittleLeaf.Api.Services.VNPay;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartService     _cartService;
        private readonly IVnPayService    _vnPayService;
        private readonly IGhnService      _ghn;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepo,
            ICartService cartService,
            IVnPayService vnPayService,
            IGhnService ghn,
            ILogger<OrderService> logger)
        {
            _orderRepo    = orderRepo;
            _cartService  = cartService;
            _vnPayService = vnPayService;
            _ghn          = ghn;
            _logger       = logger;
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

            // Mark payment received — admin must still manually confirm before GHN push
            bill.PaymentStatus = "paid";
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
                IdUser        = userId,
                AdrsFullname  = dto.AdrsFullname,
                AdrsAddress   = dto.AdrsAddress,
                AdrsPhone     = dto.AdrsPhone,
                AdrsIsDefault = dto.AdrsIsDefault,
                ProvinceId    = dto.ProvinceId,
                DistrictId    = dto.DistrictId,
                WardCode      = dto.WardCode,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
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
            address.ProvinceId    = dto.ProvinceId;
            address.DistrictId    = dto.DistrictId;
            address.WardCode      = dto.WardCode;
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
                    IdUser        = userId,
                    AdrsFullname  = dto.NewFullName,
                    AdrsAddress   = dto.NewAddress,
                    AdrsPhone     = dto.NewPhone,
                    AdrsIsDefault = false,
                    ProvinceId    = dto.NewProvinceId,
                    DistrictId    = dto.NewDistrictId,
                    WardCode      = dto.NewWardCode,
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

            int cartSubtotal = cart.Items.Sum(i => i.LineTotal);
            int totalAmount  = cartSubtotal + dto.ShippingFee;

            string paymentStatus = dto.PaymentMethod.ToUpper() == "VNPAY"
                ? "pending_vnpay"
                : "pending";

            // 3. Reserve inventory: deduct stock at order creation to prevent overselling.
            //    Wrapped in optimistic concurrency — RowVersion on Product detects concurrent writes.
            foreach (var item in cart.Items)
            {
                var product = await _orderRepo.GetProductByIdAsync(item.ProductId)
                              ?? throw new InvalidOperationException($"Sản phẩm {item.ProductId} không tìm thấy.");
                if (product.QuantityInStock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Sản phẩm '{product.ProductName}' không đủ hàng.");
                product.QuantityInStock -= item.Quantity;
            }

            // 4. Persist stock deductions before creating Bill.
            //    DbUpdateConcurrencyException fires if RowVersion mismatch (concurrent buyer).
            try
            {
                await _orderRepo.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Sản phẩm vừa hết hàng do có người khác vừa mua.");
            }

            // 5. Create Bill
            var bill = new Bill
            {
                IdUser         = userId,
                IdAdrs         = adrsId,
                DateCreated    = DateOnly.FromDateTime(DateTime.Now),
                CreatedAtTime  = DateTime.UtcNow,
                TotalAmount    = totalAmount,
                ShippingFee    = dto.ShippingFee,
                PaymentMethod  = dto.PaymentMethod.ToUpper(),
                PaymentStatus  = paymentStatus,
                IsConfirmed    = false,
                OrderStatus    = "PENDING",
                ShippingStatus = "not_fulfilled",
                Note           = dto.Note,
                UpdatedAt      = DateTime.Now
            };

            var createdBill = await _orderRepo.CreateBillAsync(bill);

            // 6. Create BillDetails from cart items
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

            // 7. Clear cart
            await _cartService.ClearCartAsync(userId);

            // 6. Return the fresh bill with address
            var result = await _orderRepo.GetBillByIdAsync(createdBill.BillId);
            return MapBillToDto(result!);
        }

        public async Task FulfillOrderAsync(int billId)
        {
            // Stock was already reserved at order creation (CreateOrderAsync).
            // This method only updates the payment timestamp.
            var bill = await _orderRepo.GetBillByIdAsync(billId)
                       ?? throw new InvalidOperationException("Order not found.");

            bill.UpdatedAt = DateTime.Now;
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
                ShippingFee    = bill.ShippingFee,
                PaymentMethod  = bill.PaymentMethod,
                PaymentStatus  = bill.PaymentStatus,
                IsConfirmed     = bill.IsConfirmed,
                OrderStatus     = bill.OrderStatus,
                ShippingStatus  = bill.ShippingStatus,
                TrackingMessage = bill.TrackingMessage,
                Note            = bill.Note,
                GhnOrderCode    = bill.GhnOrderCode,
                ShippingAddress = bill.IdAdrsNavigation == null
                    ? null
                    : await MapAddressWithNamesAsync(bill.IdAdrsNavigation),
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
            AdrsId        = a.AdrsId,
            AdrsFullname  = a.AdrsFullname,
            AdrsAddress   = a.AdrsAddress,
            AdrsPhone     = a.AdrsPhone,
            AdrsIsDefault = a.AdrsIsDefault,
            ProvinceId    = a.ProvinceId,
            DistrictId    = a.DistrictId,
            WardCode      = a.WardCode,
        };

        // Enriches an address DTO with display names from the GHN cached master data.
        private async Task<AddressDto> MapAddressWithNamesAsync(AddressList a)
        {
            var dto = MapAddressToDto(a);
            try
            {
                if (a.ProvinceId.HasValue)
                {
                    var provinces = await _ghn.GetProvincesAsync();
                    dto.ProvinceName = provinces.FirstOrDefault(p => p.ProvinceId == a.ProvinceId)?.ProvinceName;
                }
                if (a.DistrictId.HasValue)
                {
                    var districts = await _ghn.GetDistrictsAsync(a.ProvinceId ?? 0);
                    dto.DistrictName = districts.FirstOrDefault(d => d.DistrictId == a.DistrictId)?.DistrictName;
                }
                if (!string.IsNullOrEmpty(a.WardCode) && a.DistrictId.HasValue)
                {
                    var wards = await _ghn.GetWardsAsync(a.DistrictId.Value);
                    dto.WardName = wards.FirstOrDefault(w => w.WardCode == a.WardCode)?.WardName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve GHN address names for address {AdrsId}", a.AdrsId);
            }
            return dto;
        }

        private static OrderDto MapBillToDto(Bill b) => new()
        {
            BillId          = b.BillId,
            DateCreated     = b.DateCreated,
            TotalAmount     = b.TotalAmount,
            ShippingFee     = b.ShippingFee,
            PaymentMethod   = b.PaymentMethod,
            PaymentStatus   = b.PaymentStatus,
            IsConfirmed     = b.IsConfirmed,
            OrderStatus     = b.OrderStatus,
            ShippingStatus  = b.ShippingStatus,
            TrackingMessage = b.TrackingMessage,
            Note            = b.Note,
            GhnOrderCode    = b.GhnOrderCode,
            ShippingAddress = b.IdAdrsNavigation == null
                ? null
                : MapAddressToDto(b.IdAdrsNavigation)
        };
    }
}
