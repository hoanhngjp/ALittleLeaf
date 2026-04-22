using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ALittleLeaf.Tests.Controllers
{
    public class CustomerOrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrdersController    _controller;

        private const long UserId = 1;

        public CustomerOrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()) }, "mock"));

            _controller = new OrdersController(_mockOrderService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        // ORDL-01: Hiển thị danh sách đơn hàng (Có đơn)
        [Fact]
        public async Task GetOrders_HasOrders_ReturnsOkWithList()
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

        // ORDL-02: Hiển thị danh sách đơn hàng (Trống)
        [Fact]
        public async Task GetOrders_NoOrders_ReturnsEmptyList()
        {
            _mockOrderService.Setup(s => s.GetOrderHistoryAsync(UserId))
                .ReturnsAsync(new List<OrderDto>());

            var result = await _controller.GetOrders();

            var list = ApiAssert.OkValue<List<OrderDto>>(result);
            Assert.Empty(list);
        }

        // ORD-01: Xem chi tiết đơn hàng (Thành công)
        [Fact]
        public async Task GetOrder_ValidId_ReturnsOkWithDetail()
        {
            var detail = new OrderDetailDto
            {
                BillId      = 10,
                TotalAmount = 50000,
                Items       = new List<OrderLineItemDto> { new() { ProductName = "Product A", Quantity = 1 } }
            };
            _mockOrderService.Setup(s => s.GetOrderDetailAsync(10, UserId)).ReturnsAsync(detail);

            var result = await _controller.GetOrder(10);

            var dto = ApiAssert.OkValue<OrderDetailDto>(result);
            Assert.Equal(10, dto.BillId);
            Assert.Single(dto.Items);
        }

        // ORD Error: Xem đơn hàng không tồn tại (hoặc của người khác) → 404
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
