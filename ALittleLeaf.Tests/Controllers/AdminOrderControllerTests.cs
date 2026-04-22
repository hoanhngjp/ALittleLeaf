using ALittleLeaf.Api.Controllers.Admin;
using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Services.Admin;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ALittleLeaf.Tests.Controllers
{
    public class AdminOrderControllerTests
    {
        private readonly Mock<IAdminService>  _mockAdminService;
        private readonly AdminOrdersController _controller;

        public AdminOrderControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller       = new AdminOrdersController(_mockAdminService.Object);
        }

        // GET /api/admin/orders — returns paginated list
        [Fact]
        public async Task GetOrders_ReturnsOkWithPagedResult()
        {
            var paged = new PaginatedAdminResultDto<AdminOrderDto>
            {
                Items      = new List<AdminOrderDto> { new() { BillId = 100, TotalAmount = 50000 } },
                TotalItems = 1,
                Page       = 1,
                PageSize   = 10
            };
            _mockAdminService
                .Setup(s => s.GetOrdersAsync(null, null, null, null, null, "dateCreated", true, 1, 10))
                .ReturnsAsync(paged);

            var result = await _controller.GetOrders();

            var dto = ApiAssert.OkValue<PaginatedAdminResultDto<AdminOrderDto>>(result);
            Assert.Single(dto.Items);
            Assert.Equal(100, dto.Items.First().BillId);
        }

        // GET /api/admin/orders/{id} — found
        [Fact]
        public async Task GetOrder_ValidId_ReturnsOkWithDetail()
        {
            var detail = new AdminOrderDetailDto { BillId = 100, TotalAmount = 50000 };
            _mockAdminService.Setup(s => s.GetOrderByIdAsync(100)).ReturnsAsync(detail);

            var result = await _controller.GetOrder(100);

            var dto = ApiAssert.OkValue<AdminOrderDetailDto>(result);
            Assert.Equal(100, dto.BillId);
        }

        // GET /api/admin/orders/{id} — not found → 404
        [Fact]
        public async Task GetOrder_InvalidId_ReturnsNotFound()
        {
            _mockAdminService.Setup(s => s.GetOrderByIdAsync(999))
                .ReturnsAsync((AdminOrderDetailDto?)null);

            var result = await _controller.GetOrder(999);

            ApiAssert.IsNotFound(result);
        }

        // PATCH /api/admin/orders/{id}/status — updates successfully
        [Fact]
        public async Task UpdateOrderStatus_ValidId_ReturnsOkWithUpdatedOrder()
        {
            var updated = new AdminOrderDto
            {
                BillId        = 100,
                IsConfirmed   = true,
                PaymentStatus = "paid",
                ShippingStatus = "fulfilled"
            };
            var dto = new UpdateOrderStatusDto
            {
                IsConfirmed   = true,
                PaymentStatus  = "paid",
                ShippingStatus = "fulfilled"
            };
            _mockAdminService.Setup(s => s.UpdateOrderStatusAsync(100, dto)).ReturnsAsync(updated);

            var result = await _controller.UpdateOrderStatus(100, dto);

            var order = ApiAssert.OkValue<AdminOrderDto>(result);
            Assert.True(order.IsConfirmed);
            Assert.Equal("paid", order.PaymentStatus);
        }

        // PATCH /api/admin/orders/{id}/status — no fields provided → 400
        [Fact]
        public async Task UpdateOrderStatus_NoFields_ReturnsBadRequest()
        {
            var dto = new UpdateOrderStatusDto(); // all null

            var result = await _controller.UpdateOrderStatus(100, dto);

            ApiAssert.IsBadRequest(result);
            _mockAdminService.Verify(s => s.UpdateOrderStatusAsync(It.IsAny<int>(), It.IsAny<UpdateOrderStatusDto>()), Times.Never);
        }

        // PATCH /api/admin/orders/{id}/status — order not found → 404
        [Fact]
        public async Task UpdateOrderStatus_InvalidId_ReturnsNotFound()
        {
            var dto = new UpdateOrderStatusDto { ShippingStatus = "fulfilled" };
            _mockAdminService.Setup(s => s.UpdateOrderStatusAsync(999, dto))
                .ReturnsAsync((AdminOrderDto?)null);

            var result = await _controller.UpdateOrderStatus(999, dto);

            ApiAssert.IsNotFound(result);
        }
    }
}
