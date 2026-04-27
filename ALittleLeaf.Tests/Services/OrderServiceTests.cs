using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Api.Services.VNPay;
using ALittleLeaf.Api.Repositories.Cart;
using ALittleLeaf.Api.Services.Shipping;
using ALittleLeaf.Tests.Helpers;
using Moq;

namespace ALittleLeaf.Tests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly OrderService            _service;
        private readonly Mock<IVnPayService>     _mockVnPay;
        private readonly Mock<IGhnService>       _mockGhn;

        public OrderServiceTests()
        {
            _context = DbContextFactory.Create();

            var orderRepo = new OrderRepository(_context);
            var cartRepo  = new CartRepository(_context);
            var cartSvc   = new CartService(cartRepo);
            _mockVnPay    = new Mock<IVnPayService>();
            _mockGhn      = new Mock<IGhnService>();

            _service = new OrderService(orderRepo, cartSvc, _mockVnPay.Object, _mockGhn.Object,
                                        Microsoft.Extensions.Logging.Abstractions.NullLogger<OrderService>.Instance);
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task SeedCartForUser(long userId)
        {
            // Product ID 1 (Ấm Tráng Men, price=50000, stock=100) is already seeded by DbContextFactory
            var cart = new Api.Models.Cart
            {
                UserId    = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            _context.CartItems.Add(new CartItem
            {
                CartId    = cart.CartId,
                ProductId = 1,
                Quantity  = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        private async Task<int> SeedAddressForUser(long userId)
        {
            var adrs = new AddressList
            {
                IdUser        = userId,
                AdrsFullname  = "Test User",
                AdrsAddress   = "123 Street",
                AdrsPhone     = "0909000111",
                AdrsIsDefault = true,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
            };
            _context.AddressLists.Add(adrs);
            await _context.SaveChangesAsync();
            return adrs.AdrsId;
        }

        // ── CreateOrder ───────────────────────────────────────────────────────

        [Fact]
        public async Task CreateOrder_COD_CreatesBillAndClearsCart()
        {
            long userId = 10;
            await SeedCartForUser(userId);
            int adrsId  = await SeedAddressForUser(userId);

            var dto = new CreateOrderDto
            {
                AddressId     = adrsId,
                PaymentMethod = "COD"
            };

            var result = await _service.CreateOrderAsync(userId, dto);

            Assert.NotNull(result);
            Assert.Equal(100000, result.TotalAmount); // 50000 * 2
            Assert.Equal("COD", result.PaymentMethod);

            // Bill persisted — admin must confirm manually, so IsConfirmed starts false
            var savedBill = _context.Bills.FirstOrDefault(b => b.BillId == result.BillId);
            Assert.NotNull(savedBill);
            Assert.False(savedBill.IsConfirmed);
            Assert.Equal("PENDING", savedBill.OrderStatus);

            // Cart cleared immediately on order creation
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            Assert.NotNull(cart);
            Assert.Empty(_context.CartItems.Where(ci => ci.CartId == cart.CartId));
        }

        [Fact]
        public async Task CreateOrder_VnPay_CreatesBillAsPending()
        {
            long userId = 11;
            await SeedCartForUser(userId);
            int adrsId  = await SeedAddressForUser(userId);

            var dto = new CreateOrderDto { AddressId = adrsId, PaymentMethod = "VNPAY" };
            var result = await _service.CreateOrderAsync(userId, dto);

            Assert.Equal("pending_vnpay", result.PaymentStatus);
            Assert.False(result.IsConfirmed);

            // Cart is cleared immediately on order creation (payment confirmation happens via VNPay IPN)
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            Assert.NotNull(cart);
            Assert.Empty(_context.CartItems.Where(ci => ci.CartId == cart.CartId));
        }

        [Fact]
        public async Task CreateOrder_EmptyCart_ThrowsException()
        {
            long userId = 12;
            int adrsId  = await SeedAddressForUser(userId);
            // No cart seeded → GetCartAsync will create an empty one

            var dto = new CreateOrderDto { AddressId = adrsId, PaymentMethod = "COD" };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateOrderAsync(userId, dto));
        }

        // ── FulfillOrder ──────────────────────────────────────────────────────

        [Fact]
        public async Task FulfillOrder_DeductsStock()
        {
            long userId = 20;
            int adrsId  = await SeedAddressForUser(userId);

            var bill = new Bill
            {
                IdUser         = userId,
                IdAdrs         = adrsId,
                TotalAmount    = 100000,
                DateCreated    = DateOnly.FromDateTime(DateTime.Now),
                PaymentMethod  = "COD",
                PaymentStatus  = "pending",
                ShippingStatus = "not_fulfilled",
                UpdatedAt      = DateTime.Now
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            _context.BillDetails.Add(new BillDetail
            {
                IdBill    = bill.BillId,
                IdProduct = 1,
                Quantity  = 10,
                UnitPrice = 50000,
                TotalPrice = 500000
            });
            await _context.SaveChangesAsync();

            await _service.FulfillOrderAsync(bill.BillId);

            var product = await _context.Products.FindAsync(1);
            Assert.Equal(90, product!.QuantityInStock); // 100 - 10
        }

        [Fact]
        public async Task FulfillOrder_InsufficientStock_ThrowsException()
        {
            long userId = 21;
            int adrsId  = await SeedAddressForUser(userId);

            // Product 2 has stock = 10
            var bill = new Bill
            {
                IdUser         = userId,
                IdAdrs         = adrsId,
                TotalAmount    = 5000000,
                DateCreated    = DateOnly.FromDateTime(DateTime.Now),
                PaymentMethod  = "COD",
                PaymentStatus  = "pending",
                ShippingStatus = "not_fulfilled",
                UpdatedAt      = DateTime.Now
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            _context.BillDetails.Add(new BillDetail
            {
                IdBill     = bill.BillId,
                IdProduct  = 2,
                Quantity   = 20,   // > 10 in stock
                UnitPrice  = 2500000,
                TotalPrice = 50000000
            });
            await _context.SaveChangesAsync();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.FulfillOrderAsync(bill.BillId));

            Assert.Contains("out of stock", ex.Message);
        }
    }
}
