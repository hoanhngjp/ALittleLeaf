using ALittleLeaf.Areas.Admin.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Controllers
{
    public class AdminOrderControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly OrdersController _controller;

        public AdminOrderControllerTests()
        {
            _context = DbContextFactory.Create();
            _controller = new OrdersController(_context);
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        [Fact]
        public void UpdateStatus_Post_UpdatesDatabaseAndRedirects()
        {
            // Arrange
            // Tạo Bill mẫu trong DB
            var bill = new Bill
            {
                IdUser = 1,
                BillId = 100,
                IsConfirmed = false,
                PaymentStatus = "Pending",
                ShippingStatus = "Not Shipped",
                DateCreated = DateOnly.FromDateTime(DateTime.Now),

                // --- THÊM DÒNG NÀY ---
                PaymentMethod = "COD",  // <--- Bổ sung trường bắt buộc
                IdAdrs = 1 // (Tùy chọn) Nên thêm ID địa chỉ giả để dữ liệu chuẩn hơn
            };
            _context.Bills.Add(bill);
            _context.SaveChanges();

            // Act
            // Giả lập Admin confirm đơn hàng
            var result = _controller.UpdateStatus(100, true, "Paid", "Shipped");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);

            // Check DB
            var updatedBill = _context.Bills.Find(100);
            Assert.True(updatedBill.IsConfirmed);
            Assert.Equal("Paid", updatedBill.PaymentStatus);
            Assert.Equal("Shipped", updatedBill.ShippingStatus);
        }

        [Fact]
        public async Task Statistics_ReturnsViewModel()
        {
            // Act
            var result = await _controller.Statistics(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OrderStatisticsViewModel>(viewResult.Model);
            Assert.NotNull(model.RecentOrders);
            // Có thể kiểm tra thêm model.TotalOrders nếu đã seed data trong DbContextFactory
        }
    }
}