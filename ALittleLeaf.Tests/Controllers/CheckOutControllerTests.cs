using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using ALittleLeaf.Api.Data;

namespace ALittleLeaf.Tests.Controllers
{
    public class CheckOutControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IOrderService>     _mockOrderService;
        private readonly OrdersController        _controller;

        private const long UserId = 10;

        public CheckOutControllerTests()
        {
            _context          = DbContextFactory.Create();
            _mockOrderService = new Mock<IOrderService>();
            _controller       = BuildController(_mockOrderService.Object);
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        private static OrdersController BuildController(IOrderService svc)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()) }, "mock"));

            return new OrdersController(svc)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        // POST /api/orders — COD success → 201 Created

        [Fact]
        public async Task CreateOrder_CodSuccess_Returns201WithOrderDto()
        {
            var dto      = new CreateOrderDto { AddressId = 1, PaymentMethod = "COD" };
            var expected = new OrderDto { BillId = 100, TotalAmount = 50000, PaymentMethod = "COD" };

            _mockOrderService.Setup(s => s.CreateOrderAsync(UserId, dto)).ReturnsAsync(expected);

            var result = await _controller.CreateOrder(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var order   = Assert.IsType<OrderDto>(created.Value);
            Assert.Equal(100, order.BillId);

            _mockOrderService.Verify(s => s.CreateOrderAsync(UserId, dto), Times.Once);
        }

        // POST /api/orders — empty cart → 400 BadRequest

        [Fact]
        public async Task CreateOrder_EmptyCart_Returns400()
        {
            var dto = new CreateOrderDto { AddressId = 1, PaymentMethod = "COD" };

            _mockOrderService.Setup(s => s.CreateOrderAsync(UserId, dto))
                .ThrowsAsync(new InvalidOperationException("Cart is empty."));

            var result = await _controller.CreateOrder(dto);

            ApiAssert.IsBadRequest(result);
        }

        // GET /api/orders — returns list

        [Fact]
        public async Task GetOrders_ReturnsOkWithList()
        {
            var orders = new List<OrderDto>
            {
                new() { BillId = 1, TotalAmount = 100000 },
                new() { BillId = 2, TotalAmount = 200000 }
            };
            _mockOrderService.Setup(s => s.GetOrderHistoryAsync(UserId)).ReturnsAsync(orders);

            var result = await _controller.GetOrders();

            var list = ApiAssert.OkValue<List<OrderDto>>(result);
            Assert.Equal(2, list.Count);
        }

        // GET /api/orders/{id} — valid

        [Fact]
        public async Task GetOrder_ValidId_ReturnsOkWithDetail()
        {
            var detail = new OrderDetailDto { BillId = 10 };
            _mockOrderService.Setup(s => s.GetOrderDetailAsync(10, UserId)).ReturnsAsync(detail);

            var result = await _controller.GetOrder(10);

            var dto = ApiAssert.OkValue<OrderDetailDto>(result);
            Assert.Equal(10, dto.BillId);
        }

        // GET /api/orders/{id} — not found or belongs to another user

        [Fact]
        public async Task GetOrder_InvalidId_ReturnsNotFound()
        {
            _mockOrderService.Setup(s => s.GetOrderDetailAsync(999, UserId))
                .ReturnsAsync((OrderDetailDto?)null);

            var result = await _controller.GetOrder(999);

            ApiAssert.IsNotFound(result);
        }
    }
}
