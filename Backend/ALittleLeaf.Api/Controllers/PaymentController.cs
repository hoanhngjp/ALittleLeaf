using System.Security.Claims;
using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Models.VnPay;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Api.Services.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>VNPay payment endpoints.</summary>
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService  _vnPayService;
        private readonly IOrderService  _orderService;
        private readonly AlittleLeafDecorContext _context;

        public PaymentController(
            IVnPayService vnPayService,
            IOrderService orderService,
            AlittleLeafDecorContext context)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
            _context      = context;
        }

        private bool TryGetUserId(out long userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(raw, out userId);
        }

        // ── POST /api/payment/create-url ──────────────────────────────────────

        /// <summary>
        /// Generates a VNPay payment URL for a "pending_vnpay" bill.
        /// The bill must already exist (created via POST /api/orders with PaymentMethod=VNPAY).
        /// Returns JSON { billId, paymentUrl } — the frontend performs the redirect.
        /// </summary>
        [Authorize]
        [HttpPost("create-url")]
        [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] CreatePaymentUrlDto dto)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.BillId == dto.BillId && b.IdUser == userId);

            if (bill == null)
                return NotFound(new { error = "Order not found." });

            if (bill.PaymentStatus != "pending_vnpay")
                return BadRequest(new { error = "Order is not awaiting VNPay payment." });

            var model = new PaymentInformationModel
            {
                Amount           = bill.TotalAmount,
                Name             = $"KhachHang{bill.IdUser}",
                OrderDescription = $"Thanh toan don hang {bill.BillId}",
                OrderType        = "other"
            };

            var txnRef = $"{bill.BillId}_{DateTime.UtcNow.Ticks}";
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, txnRef);
            return Ok(new PaymentResponseDto { BillId = bill.BillId, PaymentUrl = url });
        }

        // ── GET /api/payment/vnpay-callback ───────────────────────────────────

        /// <summary>
        /// VNPay redirects the customer's browser here after payment.
        /// Also acts as a fallback if the IPN webhook did not reach the local instance.
        /// Delegates to the idempotent ConfirmPaymentAsync — safe to be called after IPN.
        /// Returns JSON so the React frontend can navigate to the result page.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("vnpay-callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VnpayCallback()
        {
            var result = await _orderService.ConfirmPaymentAsync(Request.Query);

            if (!result.Success)
                return BadRequest(new
                {
                    success         = false,
                    message         = result.Message,
                    vnpResponseCode = result.VnpResponseCode
                });

            return Ok(new { success = true, billId = result.BillId, alreadyPaid = result.AlreadyPaid });
        }

        // ── GET /api/payment/vnpay-ipn ────────────────────────────────────────

        /// <summary>
        /// Server-to-server IPN webhook from VNPay.
        /// Must return the exact JSON format VNPay expects: {"RspCode":"00","Message":"..."}.
        /// Delegates to the idempotent ConfirmPaymentAsync — safe to call before Return URL.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnpayIpn()
        {
            try
            {
                var result = await _orderService.ConfirmPaymentAsync(Request.Query);

                return result.Success
                    ? Ok(new { RspCode = "00", Message = "Confirm Success" })
                    : Ok(new { RspCode = result.VnpResponseCode, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { RspCode = "99", Message = ex.Message });
            }
        }

        // ── POST /api/payment/retry/{billId} ──────────────────────────────────

        /// <summary>Generates a new VNPay URL for a previously unpaid order.</summary>
        [Authorize]
        [HttpPost("retry/{billId:int}")]
        [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetryPayment(int billId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { error = "Invalid token claims." });

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.BillId == billId && b.IdUser == userId);

            if (bill == null)
                return NotFound(new { error = "Order not found." });

            if (bill.PaymentStatus != "pending_vnpay")
                return BadRequest(new { error = "This order cannot be retried." });

            var model = new PaymentInformationModel
            {
                Amount           = bill.TotalAmount,
                Name             = $"KhachHang{bill.IdUser}",
                OrderDescription = $"Thanh toan lai don hang {bill.BillId}",
                OrderType        = "other"
            };

            var txnRef = $"{bill.BillId}_{DateTime.UtcNow.Ticks}";
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, txnRef);
            return Ok(new PaymentResponseDto { BillId = bill.BillId, PaymentUrl = url });
        }
    }
}
