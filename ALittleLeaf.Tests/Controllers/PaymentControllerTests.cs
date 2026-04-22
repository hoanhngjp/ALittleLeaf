using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Models.VnPay;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Api.Services.VNPay;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Security.Claims;
using ALittleLeaf.Api.Data;

namespace ALittleLeaf.Tests.Controllers
{
    public class PaymentControllerTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IVnPayService>     _mockVnPay;
        private readonly Mock<IOrderService>     _mockOrderService;
        private readonly PaymentController       _controller;

        private const long UserId = 1;

        public PaymentControllerTests()
        {
            _context          = DbContextFactory.Create();
            _mockVnPay        = new Mock<IVnPayService>();
            _mockOrderService = new Mock<IOrderService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()) }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };
            // Provide an empty query string so Request.Query never throws
            httpContext.Request.QueryString = QueryString.Empty;

            _controller = new PaymentController(_mockVnPay.Object, _mockOrderService.Object, _context)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        // ── POST /api/payment/create-url ──────────────────────────────────────

        // TC-05: Tạo URL thanh toán VNPay cho đơn hàng hợp lệ → 200 OK với paymentUrl
        [Fact]
        public async Task CreatePaymentUrl_ValidBill_ReturnsOkWithUrl()
        {
            // Seed a pending_vnpay bill belonging to UserId=1
            var bill = new Api.Models.Bill
            {
                IdUser         = UserId,
                IdAdrs         = 1,
                TotalAmount    = 100000,
                PaymentStatus  = "pending_vnpay",
                PaymentMethod  = "VNPAY",
                ShippingStatus = "not_fulfilled",
                DateCreated    = DateOnly.FromDateTime(DateTime.Now),
                UpdatedAt      = DateTime.Now
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            _mockVnPay
                .Setup(s => s.CreatePaymentUrl(It.IsAny<PaymentInformationModel>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
                .Returns("https://sandbox.vnpayment.vn/paymentv2/fake-url");

            var dto    = new CreatePaymentUrlDto { BillId = bill.BillId };
            var result = await _controller.CreatePaymentUrl(dto);

            var response = ApiAssert.OkValue<PaymentResponseDto>(result);
            Assert.Equal(bill.BillId, response.BillId);
            Assert.Contains("vnpayment.vn", response.PaymentUrl);
        }

        // CreatePaymentUrl: bill not found (or belongs to another user) → 404
        [Fact]
        public async Task CreatePaymentUrl_BillNotFound_ReturnsNotFound()
        {
            var dto    = new CreatePaymentUrlDto { BillId = 9999 };
            var result = await _controller.CreatePaymentUrl(dto);

            ApiAssert.IsNotFound(result);
        }

        // CreatePaymentUrl: bill is not pending_vnpay → 400
        [Fact]
        public async Task CreatePaymentUrl_BillAlreadyPaid_ReturnsBadRequest()
        {
            var bill = new Api.Models.Bill
            {
                IdUser         = UserId,
                IdAdrs         = 1,
                TotalAmount    = 50000,
                PaymentStatus  = "paid",      // already confirmed
                PaymentMethod  = "VNPAY",
                ShippingStatus = "fulfilled",
                DateCreated    = DateOnly.FromDateTime(DateTime.Now),
                UpdatedAt      = DateTime.Now
            };
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            var dto    = new CreatePaymentUrlDto { BillId = bill.BillId };
            var result = await _controller.CreatePaymentUrl(dto);

            ApiAssert.IsBadRequest(result);
        }

        // ── GET /api/payment/vnpay-callback ───────────────────────────────────

        // TC-06: Callback thành công (VNPay trả về Success=true) → 200 OK { success=true }
        [Fact]
        public async Task VnpayCallback_SuccessResponse_ReturnsOkWithSuccessTrue()
        {
            _mockOrderService
                .Setup(s => s.ConfirmPaymentAsync(It.IsAny<IQueryCollection>()))
                .ReturnsAsync(new PaymentResultDto
                {
                    Success         = true,
                    BillId          = 300,
                    VnpResponseCode = "00",
                    AlreadyPaid     = false
                });

            var result = await _controller.VnpayCallback();

            // Returns 200 OK with an anonymous object — check status code only
            ApiAssert.IsOk(result);
        }

        // TC-07/08: Callback thất bại / hủy → 400 BadRequest
        [Fact]
        public async Task VnpayCallback_FailureResponse_ReturnsBadRequest()
        {
            _mockOrderService
                .Setup(s => s.ConfirmPaymentAsync(It.IsAny<IQueryCollection>()))
                .ReturnsAsync(new PaymentResultDto
                {
                    Success         = false,
                    VnpResponseCode = "24",
                    Message         = "Transaction cancelled."
                });

            var result = await _controller.VnpayCallback();

            ApiAssert.IsBadRequest(result);
        }

        // ── GET /api/payment/vnpay-ipn ────────────────────────────────────────

        // IPN Success → 200 OK with { RspCode="00" }
        [Fact]
        public async Task VnpayIpn_SuccessPayment_ReturnsRspCode00()
        {
            _mockOrderService
                .Setup(s => s.ConfirmPaymentAsync(It.IsAny<IQueryCollection>()))
                .ReturnsAsync(new PaymentResultDto { Success = true, BillId = 300, VnpResponseCode = "00" });

            var result = await _controller.VnpayIpn();

            // IPN always returns 200; VNPay reads RspCode from the body
            var ok = ApiAssert.IsOk(result);
            // Verify the body contains RspCode via reflection on the anonymous type
            var value = ok.Value!;
            var rspCode = value.GetType().GetProperty("RspCode")?.GetValue(value)?.ToString();
            Assert.Equal("00", rspCode);
        }

        // IPN Failure → 200 OK with { RspCode != "00" }
        [Fact]
        public async Task VnpayIpn_FailurePayment_ReturnsNon00RspCode()
        {
            _mockOrderService
                .Setup(s => s.ConfirmPaymentAsync(It.IsAny<IQueryCollection>()))
                .ReturnsAsync(new PaymentResultDto { Success = false, VnpResponseCode = "24", Message = "Cancelled" });

            var result = await _controller.VnpayIpn();

            var ok = ApiAssert.IsOk(result);
            var value = ok.Value!;
            var rspCode = value.GetType().GetProperty("RspCode")?.GetValue(value)?.ToString();
            Assert.NotEqual("00", rspCode);
        }

        // IPN Exception → 200 OK with { RspCode="99" }
        [Fact]
        public async Task VnpayIpn_ServiceThrows_ReturnsRspCode99()
        {
            _mockOrderService
                .Setup(s => s.ConfirmPaymentAsync(It.IsAny<IQueryCollection>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.VnpayIpn();

            var ok = ApiAssert.IsOk(result);
            var value = ok.Value!;
            var rspCode = value.GetType().GetProperty("RspCode")?.GetValue(value)?.ToString();
            Assert.Equal("99", rspCode);
        }
    }
}
