using ALittleLeaf.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Services; // IAuthService
using ALittleLeaf.Services.Auth;
using ALittleLeaf.Services.Order;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Controllers
{
    public class CustomerOrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AccountController _controller;

        public CustomerOrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockAuthService = new Mock<IAuthService>();

            // Mock User Login (UserId = 1)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", "1"),
            }, "mock"));

            _controller = new AccountController(_mockAuthService.Object, _mockOrderService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        // ORDL-01: Hiển thị danh sách đơn hàng (Có đơn)
        [Fact]
        public async Task Index_HasOrders_ReturnsViewWithList()
        {
            // Arrange
            var orders = new List<Bill>
            {
                new Bill { BillId = 1, TotalAmount = 100000 },
                new Bill { BillId = 2, TotalAmount = 200000 }
            };
            _mockOrderService.Setup(s => s.GetOrderHistoryAsync(1)).ReturnsAsync(orders);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Bill>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        // ORDL-02: Hiển thị danh sách đơn hàng (Trống)
        [Fact]
        public async Task Index_NoOrders_ReturnsEmptyList()
        {
            // Arrange
            _mockOrderService.Setup(s => s.GetOrderHistoryAsync(1)).ReturnsAsync(new List<Bill>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Bill>>(viewResult.Model);
            Assert.Empty(model);
        }

        // ORD-01: Xem chi tiết đơn hàng (Thành công)
        [Fact]
        public async Task OrderDetails_ValidId_ReturnsViewWithDetails()
        {
            // Arrange
            int billId = 10;
            var bill = new Bill { BillId = 10, TotalAmount = 50000 };
            var details = new List<OrderDetailViewModel>
            {
                new OrderDetailViewModel { ProductName = "Product A", Quantity = 1 }
            };

            _mockOrderService.Setup(s => s.GetBillByIdAsync(billId, 1)).ReturnsAsync(bill);
            _mockOrderService.Setup(s => s.GetBillDetailsAsync(billId)).ReturnsAsync(details);

            // Act
            var result = await _controller.OrderDetails(billId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<OrderDetailViewModel>>(viewResult.Model); // View nhận details
            Assert.Single(model);

            // Kiểm tra ViewBag chứa Bill info
            Assert.NotNull(_controller.ViewBag.Bill);
            var billInViewBag = (Bill)_controller.ViewBag.Bill;
            Assert.Equal(10, billInViewBag.BillId);
        }

        // ORD Error: Xem đơn hàng không tồn tại (hoặc của người khác)
        [Fact]
        public async Task OrderDetails_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockOrderService.Setup(s => s.GetBillByIdAsync(999, 1)).ReturnsAsync((Bill)null);

            // Act
            var result = await _controller.OrderDetails(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}