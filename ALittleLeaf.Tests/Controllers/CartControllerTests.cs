using ALittleLeaf.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Cart;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Controllers
{
    public class CartControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<ICartService> _mockCartService;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            // 1. Setup InMemory DB (Để Controller check tồn kho từ _context.Products)
            _context = DbContextFactory.Create();

            // 2. Setup Mock Service
            _mockCartService = new Mock<ICartService>();

            // 3. Setup Controller
            _controller = new CartController(_mockCartService.Object, _context)
            {
                // Setup TempData để check thông báo lỗi
                TempData = new TempDataDictionary(new Microsoft.AspNetCore.Http.DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        // TC01: Thêm vào giỏ thành công
        [Fact]
        public async Task AddToCart_ValidStock_RedirectsToCart()
        {
            // Arrange (SP ID 1, Tồn kho 100)
            int productId = 1;
            int quantity = 2;

            _mockCartService.Setup(s => s.AddToCartAsync(productId, quantity))
                .ReturnsAsync(new CartUpdateResult { Success = true });

            // Act
            var result = await _controller.AddToCart(productId, quantity);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Cart", redirectResult.ControllerName);
        }

        // TC01 (Stock): Thêm quá số lượng tồn kho
        [Fact]
        public async Task AddToCart_ExceedStock_RedirectsToProductWithTempData()
        {
            // Arrange (SP ID 2, Tồn kho 10 - xem DbMock)
            int productId = 2;
            int quantity = 11; // Mua 11 > 10

            // Act
            var result = await _controller.AddToCart(productId, quantity);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            // Check redirect về trang Product
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Product", redirectResult.ControllerName);

            // Check TempData hiển thị lỗi
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Contains("Kho chỉ còn 10 sản phẩm", _controller.TempData["Error"].ToString());

            // Đảm bảo Service KHÔNG được gọi
            _mockCartService.Verify(s => s.AddToCartAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        // TC02: Update số lượng thành công -> Trả JSON
        [Fact]
        public void UpdateCartItem_ValidQty_ReturnsJsonSuccess()
        {
            // Arrange
            int productId = 1;
            int quantity = 5;

            _mockCartService.Setup(s => s.UpdateCartItem(productId, quantity))
                .Returns(new CartUpdateResult { Success = true, TotalPrice = 500 });

            // Act
            var result = _controller.UpdateCartItem(productId, quantity);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic data = jsonResult.Value;
            // Lưu ý: data ở đây là CartUpdateResult object
            // Dùng Reflection hoặc cast để check nếu cần thiết, hoặc tin tưởng type JsonResult
            Assert.True(data.Success);
        }

        // TC03: Update số lượng < 1 -> Trả JSON lỗi
        [Fact]
        public void UpdateCartItem_QuantityLessThanOne_ReturnsJsonError()
        {
            // Act
            var result = _controller.UpdateCartItem(1, 0);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);

            // --- SỬA ĐOẠN NÀY ---
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Cho phép hiển thị tiếng Việt
            };
            var jsonString = JsonSerializer.Serialize(jsonResult.Value, options);
            // --------------------

            Assert.Contains("Số lượng phải lớn hơn 0", jsonString);
        }

        // Check Stock khi Update
        [Fact]
        public void UpdateCartItem_ExceedStock_ReturnsJsonError()
        {
            // Arrange
            int productId = 2;
            int quantity = 20;

            // Act
            var result = _controller.UpdateCartItem(productId, quantity);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);

            // --- SỬA ĐOẠN NÀY ---
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Cho phép hiển thị tiếng Việt
            };
            var jsonString = JsonSerializer.Serialize(jsonResult.Value, options);
            // --------------------

            Assert.Contains("Kho chỉ còn 10 sản phẩm", jsonString);
        }

        // TC04: Xóa sản phẩm
        [Fact]
        public void RemoveFromCart_CallsService_ReturnsJson()
        {
            // Arrange
            _mockCartService.Setup(s => s.RemoveFromCart(1))
                .Returns(new CartUpdateResult { Success = true });

            // Act
            var result = _controller.RemoveFromCart(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic data = jsonResult.Value;
            Assert.True(data.Success);
        }
    }
}