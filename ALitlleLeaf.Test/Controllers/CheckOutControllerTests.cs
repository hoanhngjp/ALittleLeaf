using ALittleLeaf.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Order;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Controllers
{
    public class CheckOutControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context; // Cần thiết vì Controller của bạn có Inject Context
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ISession> _mockSession;
        private readonly CheckOutController _controller;
        private Dictionary<string, byte[]> _sessionStore;

        public CheckOutControllerTests()
        {
            _context = DbContextFactory.Create();
            _mockOrderService = new Mock<IOrderService>();

            // Mock Session
            _sessionStore = new Dictionary<string, byte[]>();
            _mockSession = new Mock<ISession>();
            _mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => _sessionStore[key] = value);
            _mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) => _sessionStore.TryGetValue(key, out value));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

            _controller = new CheckOutController(_mockOrderService.Object, _context)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                TempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>())
            };
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        // TC01, TC03: Lưu thông tin ship -> Redirect Payment
        [Fact]
        public void SaveShippingInfo_ValidData_SavesToSessionAndRedirects()
        {
            // Act
            var result = _controller.SaveShippingInfo("new", "Nguyen Van A", "0909123456", "123 Street");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Payment", redirectResult.ActionName);
            Assert.Equal("CheckOut", redirectResult.ControllerName);

            // Kiểm tra Session
            Assert.True(_sessionStore.ContainsKey("BillingFullName"));
            Assert.Equal("Nguyen Van A", Encoding.UTF8.GetString(_sessionStore["BillingFullName"]));
        }

        // PlaceCodOrder: Đặt hàng COD thành công
        [Fact]
        public async Task PlaceCodOrder_Success_RedirectsToAccount()
        {
            // Arrange
            var mockBill = new Bill { BillId = 100, TotalAmount = 50000 };

            _mockOrderService.Setup(s => s.CreateOrderFromSessionAsync("cod", "pending"))
                .ReturnsAsync(mockBill);

            // Act
            var result = await _controller.PlaceCodOrder("cod");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);

            // Kiểm tra Service được gọi đúng thứ tự
            _mockOrderService.Verify(s => s.CreateOrderFromSessionAsync("cod", "pending"), Times.Once);
            _mockOrderService.Verify(s => s.FulfillOrderAsync(100), Times.Once);
        }

        // PlaceCodOrder: Lỗi (Ví dụ hết hàng) -> Redirect về Cart
        [Fact]
        public async Task PlaceCodOrder_Error_RedirectsToCart()
        {
            // Arrange
            _mockOrderService.Setup(s => s.CreateOrderFromSessionAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Out of stock"));

            // Act
            var result = await _controller.PlaceCodOrder("cod");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Cart", redirectResult.ControllerName);
        }
    }
}