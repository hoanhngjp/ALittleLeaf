using ALittleLeaf.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Order;
using ALittleLeaf.Services.VNPay;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ALittleLeaf.Tests.Controllers
{
    public class PaymentControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IVnPayService> _mockVnPayService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            _context = DbContextFactory.Create();
            _mockVnPayService = new Mock<IVnPayService>();
            _mockOrderService = new Mock<IOrderService>();

            _controller = new PaymentController(_mockVnPayService.Object, _mockOrderService.Object, _context);

            // --- SETUP HTTP CONTEXT & QUERY ---
            var mockRequest = new Mock<HttpRequest>();
            var mockHttpContext = new Mock<HttpContext>();

            // SỬA: Khởi tạo QueryCollection với Dictionary rỗng để tránh NullReference
            var emptyQuery = new QueryCollection(new Dictionary<string, StringValues>());
            mockRequest.Setup(r => r.Query).Returns(emptyQuery);

            // Gán Request vào Context
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            // Gán Context vào Controller
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Bổ sung thêm TempData để an toàn (tránh lỗi nếu Controller base có dùng)
            _controller.TempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>());
        }
        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        // TC05: Tạo URL thanh toán VNPay
        [Fact]
        public async Task CreatePaymentUrl_ValidOrder_RedirectsToVnPay()
        {
            // Arrange
            var mockBill = new Bill { BillId = 200, TotalAmount = 100000, IdUser = 1 };
            _mockOrderService.Setup(s => s.CreateOrderFromSessionAsync("VNPAY", "pending_vnpay"))
                .ReturnsAsync(mockBill);

            _mockVnPayService.Setup(s => s.CreatePaymentUrl(It.IsAny<PaymentInformationModel>(), It.IsAny<HttpContext>(), "200"))
                .Returns("https://sandbox.vnpayment.vn/paymentv2/...");

            // Act
            var result = await _controller.CreatePaymentUrl();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Contains("vnpayment.vn", redirectResult.Url);
        }

        // TC06: Thanh toán thành công (Callback)
        [Fact]
        public async Task PaymentCallbackVnpay_SuccessCode_UpdatesOrderAndShowSuccessView()
        {
            // Arrange
            // 1. Tạo đơn hàng Pending trong DB
            var bill = new Bill
            {
                BillId = 300,
                PaymentStatus = "pending_vnpay",
                IsConfirmed = false,
                // Thêm các field required để tránh lỗi DbUpdateException
                PaymentMethod = "VNPAY",
                ShippingStatus = "Not Shipped",
                IdAdrs = 1,
                DateCreated = DateOnly.FromDateTime(DateTime.Now),
                IdUser = 1
            };
            _context.Bills.Add(bill);
            _context.SaveChanges();

            // 2. Mock VNPay response code 00 (Success)
            var mockResponse = new PaymentResponseModel
            {
                Success = true,
                VnPayResponseCode = "00",
                OrderId = "300" // Khớp BillId
            };
            _mockVnPayService.Setup(s => s.PaymentExecute(It.IsAny<IQueryCollection>()))
                .Returns(mockResponse);

            // Act
            var result = await _controller.PaymentCallbackVnpay();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PaymentSuccess", viewResult.ViewName);

            // Kiểm tra DB đã cập nhật chưa
            var updatedBill = _context.Bills.Find(300);
            Assert.Equal("paid", updatedBill.PaymentStatus);
            Assert.True(updatedBill.IsConfirmed);

            // Kiểm tra Service trừ kho được gọi
            _mockOrderService.Verify(s => s.FulfillOrderAsync(300), Times.Once);
        }

        // TC07, TC08: Thanh toán thất bại/Hủy (Callback)
        [Fact]
        public async Task PaymentCallbackVnpay_FailCode_ReturnsFailView()
        {
            // Arrange
            // Mock VNPay response code 24 (Hủy giao dịch) hoặc khác 00
            var mockResponse = new PaymentResponseModel
            {
                Success = true,
                VnPayResponseCode = "24", // Mã lỗi
                OrderId = "300"
            };
            _mockVnPayService.Setup(s => s.PaymentExecute(It.IsAny<IQueryCollection>()))
                .Returns(mockResponse);

            // Act
            var result = await _controller.PaymentCallbackVnpay();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PaymentFail", viewResult.ViewName);

            // Đảm bảo không gọi trừ kho
            _mockOrderService.Verify(s => s.FulfillOrderAsync(It.IsAny<int>()), Times.Never);
        }
    }
}