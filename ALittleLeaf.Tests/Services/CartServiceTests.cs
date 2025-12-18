using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Cart;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Services
{
    public class CartServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ISession> _mockSession;
        private readonly CartService _service;

        // Giả lập kho chứa Session (Key - Value)
        private Dictionary<string, byte[]> _sessionStore;

        public CartServiceTests()
        {
            // 1. Setup DB (Để lấy thông tin Product khi AddToCart)
            _context = DbContextFactory.Create();

            // 2. Setup Mock Session (Dictionary Backed)
            _sessionStore = new Dictionary<string, byte[]>();
            _mockSession = new Mock<ISession>();

            // Giả lập Set (Lưu)
            _mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => _sessionStore[key] = value);

            // Giả lập TryGetValue (Lấy)
            _mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) => _sessionStore.TryGetValue(key, out value));

            // 3. Setup HttpContext
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Session).Returns(_mockSession.Object);
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

            // 4. Init Service
            _service = new CartService(_context, _mockHttpContextAccessor.Object);
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        // Helper: Ghi dữ liệu vào Session giả trước khi test
        private void SeedCart(List<CartItemViewModel> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            _sessionStore["Cart"] = Encoding.UTF8.GetBytes(json);
        }

        // --- TEST CASES ---

        [Fact]
        public async Task AddToCartAsync_NewItem_AddsToSession()
        {
            // Arrange (Sản phẩm ID 1 đã có trong DbMock)
            int productId = 1;
            int quantity = 2;

            // Act
            var result = await _service.AddToCartAsync(productId, quantity);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(100000, result.TotalPrice); // 50k * 2

            // Kiểm tra Session đã lưu chưa
            Assert.True(_sessionStore.ContainsKey("Cart"));
        }

        [Fact]
        public async Task AddToCartAsync_ExistingItem_UpdatesQuantity()
        {
            // Arrange: Giỏ hàng đã có 1 cái Ấm
            var initialCart = new List<CartItemViewModel>
            {
                new CartItemViewModel { ProductId = 1, Quantity = 1, ProductPrice = 50000 }
            };
            SeedCart(initialCart);

            // Act: Thêm tiếp 2 cái nữa
            var result = await _service.AddToCartAsync(1, 2);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(3, result.TotalItems); // 1 + 2 = 3
            Assert.Equal(150000, result.TotalPrice); // 50k * 3
        }

        [Fact]
        public void UpdateCartItem_ValidQty_UpdatesTotals()
        {
            // Arrange
            var initialCart = new List<CartItemViewModel>
            {
                new CartItemViewModel { ProductId = 1, Quantity = 1, ProductPrice = 50000 }
            };
            SeedCart(initialCart);

            // Act: Update lên 5 cái
            var result = _service.UpdateCartItem(1, 5);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(5, result.TotalItems);
            Assert.Equal(250000, result.TotalPrice);
            Assert.Equal(250000, result.LineItemTotal); // Giá của dòng đó
        }

        [Fact]
        public void RemoveFromCart_ExistingItem_RemovesIt()
        {
            // Arrange
            var initialCart = new List<CartItemViewModel>
            {
                new CartItemViewModel { ProductId = 1, Quantity = 1 }
            };
            SeedCart(initialCart);

            // Act
            var result = _service.RemoveFromCart(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(0, result.TotalItems);
        }
    }
}