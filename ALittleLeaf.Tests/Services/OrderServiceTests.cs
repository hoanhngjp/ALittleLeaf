using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Order;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Cần để Mock User ID
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ISession> _mockSession;
        private readonly OrderService _service;
        private Dictionary<string, byte[]> _sessionStore;

        public OrderServiceTests()
        {
            _context = DbContextFactory.Create();
            _sessionStore = new Dictionary<string, byte[]>();
            _mockSession = new Mock<ISession>();

            // 1. Setup Mock Session
            _mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => _sessionStore[key] = value);
            _mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) => _sessionStore.TryGetValue(key, out value));
            _mockSession.Setup(s => s.Remove(It.IsAny<string>()))
                .Callback<string>(key => _sessionStore.Remove(key));

            // 2. Setup Mock User (Để hàm GetUserId() hoạt động)
            var claims = new List<Claim>
            {
                new Claim("Id", "1"), // Giả sử User ID = 1
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // 3. Setup HttpContext
            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Session).Returns(_mockSession.Object);
            mockContext.Setup(c => c.User).Returns(claimsPrincipal); // Gán User giả vào Context

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

            _service = new OrderService(_context, _mockHttpContextAccessor.Object);
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        // Helper: Ghi dữ liệu vào Session giả
        private void SeedSessionData()
        {
            // Cart Data
            var cart = new List<CartItemViewModel>
            {
                new CartItemViewModel { ProductId = 1, Quantity = 2, ProductPrice = 50000 }
            };
            _sessionStore["Cart"] = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cart));

            // Billing Data
            _sessionStore["BillingAdrsId"] = Encoding.UTF8.GetBytes("new"); // Case tạo địa chỉ mới
            _sessionStore["BillingFullName"] = Encoding.UTF8.GetBytes("Test User");
            _sessionStore["BillingAddress"] = Encoding.UTF8.GetBytes("123 Street");
            _sessionStore["BillingPhone"] = Encoding.UTF8.GetBytes("0909000111");
            _sessionStore["BillNote"] = Encoding.UTF8.GetBytes("Test Note");
        }

        [Fact]
        public async Task CreateOrderFromSessionAsync_ValidSession_CreatesBillAndDetails()
        {
            // Arrange
            SeedSessionData();

            // Act
            var bill = await _service.CreateOrderFromSessionAsync("COD", "Pending");

            // Assert
            Assert.NotNull(bill);
            Assert.Equal(100000, bill.TotalAmount); // 50k * 2
            Assert.Equal("COD", bill.PaymentMethod);

            // Kiểm tra DB
            var savedBill = _context.Bills.FirstOrDefault(b => b.BillId == bill.BillId);
            Assert.NotNull(savedBill);

            var savedDetails = _context.BillDetails.Where(bd => bd.IdBill == bill.BillId).ToList();
            Assert.Single(savedDetails);
            Assert.Equal(1, savedDetails[0].IdProduct);

            // Kiểm tra Address được tạo mới (vì BillingAdrsId = "new")
            var savedAddress = _context.AddressLists.FirstOrDefault(a => a.AdrsAddress == "123 Street");
            Assert.NotNull(savedAddress);
        }

        [Fact]
        public async Task FulfillOrderAsync_ValidBill_DeductsStockAndClearsSession()
        {
            // Arrange
            // 1. Tạo Bill & Detail trong DB
            var bill = new Bill
            {
                IdUser = 1,
                TotalAmount = 100000,
                DateCreated = DateOnly.FromDateTime(DateTime.Now),

                // --- THÊM CÁC DÒNG NÀY ---
                PaymentMethod = "COD",
                PaymentStatus = "Pending",
                ShippingStatus = "Unfulfilled",
                IdAdrs = 1
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync(); 

            _context.BillDetails.Add(new BillDetail
            {
                IdBill = bill.BillId,
                IdProduct = 1, // Ấm Tráng Men (Tồn kho 100 - từ DbMock)
                Quantity = 10
            });
            await _context.SaveChangesAsync();

            // 2. Setup Session có dữ liệu (để check xem có bị xóa không)
            SeedSessionData();

            // Act
            await _service.FulfillOrderAsync(bill.BillId);

            // Assert
            // 1. Kiểm tra tồn kho
            var product = await _context.Products.FindAsync(1);
            Assert.Equal(90, product.QuantityInStock); // 100 - 10 = 90

            // 2. Kiểm tra Session bị xóa
            Assert.False(_sessionStore.ContainsKey("Cart"));
            Assert.False(_sessionStore.ContainsKey("BillingFullName"));
        }

        [Fact]
        public async Task FulfillOrderAsync_NotEnoughStock_ThrowsException()
        {
            // Arrange
            // SP ID 2 (Bếp Từ) có tồn kho = 10 (theo DbMock)
            var bill = new Bill
            {
                IdUser = 1,
                TotalAmount = 5000000,
                DateCreated = DateOnly.FromDateTime(DateTime.Now),

                // --- THÊM CÁC DÒNG NÀY ---
                PaymentMethod = "Online",
                PaymentStatus = "Paid",
                ShippingStatus = "Unfulfilled",
                IdAdrs = 1
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            _context.BillDetails.Add(new BillDetail
            {
                IdBill = bill.BillId,
                IdProduct = 2,
                Quantity = 20 // Mua 20 > 10
            });
            await _context.SaveChangesAsync();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.FulfillOrderAsync(bill.BillId));
            Assert.Contains("không đủ hàng", ex.Message);
        }
    }
}