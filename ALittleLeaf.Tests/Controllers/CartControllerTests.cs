using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Cart;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using ALittleLeaf.Api.Data;

namespace ALittleLeaf.Tests.Controllers
{
    public class CartControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<ICartService>      _mockCartService;
        private readonly CartController          _controller;

        private const long UserId = 42;

        public CartControllerTests()
        {
            _context         = DbContextFactory.Create();
            _mockCartService = new Mock<ICartService>();
            _controller      = BuildController(_mockCartService.Object);
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        private static CartController BuildController(ICartService svc)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()) }, "mock"));

            return new CartController(svc)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        // ── GET /api/cart ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetCart_ValidJwt_ReturnsOkWithCartDto()
        {
            var expected = new CartDto { CartId = 1 };
            _mockCartService.Setup(s => s.GetCartAsync(UserId)).ReturnsAsync(expected);

            var result = await _controller.GetCart();

            var dto = ApiAssert.OkValue<CartDto>(result);
            Assert.Equal(1, dto.CartId);
        }

        // ── POST /api/cart/items ──────────────────────────────────────────────

        [Fact]
        public async Task AddItem_ValidDto_ReturnsOkWithUpdatedCart()
        {
            var dto      = new AddToCartDto { ProductId = 1, Quantity = 2 };
            var expected = new CartDto { CartId = 1 };
            _mockCartService.Setup(s => s.AddItemAsync(UserId, 1, 2)).ReturnsAsync(expected);

            var result = await _controller.AddItem(dto);

            ApiAssert.OkValue<CartDto>(result);
            _mockCartService.Verify(s => s.AddItemAsync(UserId, 1, 2), Times.Once);
        }

        // ── PUT /api/cart/items/{productId} ───────────────────────────────────

        [Fact]
        public async Task UpdateItem_ItemExists_ReturnsOk()
        {
            var dto      = new UpdateCartItemDto { Quantity = 5 };
            var expected = new CartDto { CartId = 1 };
            _mockCartService.Setup(s => s.UpdateItemAsync(UserId, 1, 5)).ReturnsAsync(expected);

            var result = await _controller.UpdateItem(1, dto);

            ApiAssert.OkValue<CartDto>(result);
        }

        [Fact]
        public async Task UpdateItem_ItemNotInCart_ReturnsNotFound()
        {
            var dto = new UpdateCartItemDto { Quantity = 5 };
            _mockCartService.Setup(s => s.UpdateItemAsync(UserId, 999, 5))
                .ReturnsAsync((CartDto?)null);

            var result = await _controller.UpdateItem(999, dto);

            ApiAssert.IsNotFound(result);
        }

        // ── DELETE /api/cart/items/{productId} ────────────────────────────────

        [Fact]
        public async Task RemoveItem_ItemExists_ReturnsOk()
        {
            var expected = new CartDto { CartId = 1 };
            _mockCartService.Setup(s => s.RemoveItemAsync(UserId, 1)).ReturnsAsync(expected);

            var result = await _controller.RemoveItem(1);

            ApiAssert.OkValue<CartDto>(result);
        }

        [Fact]
        public async Task RemoveItem_ItemNotInCart_ReturnsNotFound()
        {
            _mockCartService.Setup(s => s.RemoveItemAsync(UserId, 999))
                .ReturnsAsync((CartDto?)null);

            var result = await _controller.RemoveItem(999);

            ApiAssert.IsNotFound(result);
        }

        // ── DELETE /api/cart ──────────────────────────────────────────────────

        [Fact]
        public async Task ClearCart_ReturnsOkWithEmptyCart()
        {
            var expected = new CartDto { CartId = 1 };
            _mockCartService.Setup(s => s.ClearCartAsync(UserId)).ReturnsAsync(expected);

            var result = await _controller.ClearCart();

            ApiAssert.OkValue<CartDto>(result);
        }
    }
}
