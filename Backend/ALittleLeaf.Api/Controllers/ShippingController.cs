using ALittleLeaf.Api.DTOs.Shipping;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Services.Shipping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Public endpoints for GHN address master data (province / district / ward).
    /// No authentication required — these are lookup lists for the address form.
    /// </summary>
    [ApiController]
    [Route("api/shipping")]
    public class ShippingController : ControllerBase
    {
        private readonly IGhnService _ghn;
        private readonly IOrderRepository _orderRepo;
        private readonly ILogger<ShippingController> _logger;

        public ShippingController(IGhnService ghn, IOrderRepository orderRepo, ILogger<ShippingController> logger)
        {
            _ghn       = ghn;
            _orderRepo = orderRepo;
            _logger    = logger;
        }

        /// <summary>Returns all GHN provinces (cached 24 h).</summary>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
            => Ok(await _ghn.GetProvincesAsync());

        /// <summary>Returns districts for the given province (cached 24 h per province).</summary>
        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts([FromQuery] int provinceId)
        {
            if (provinceId <= 0)
                return BadRequest("provinceId is required.");

            return Ok(await _ghn.GetDistrictsAsync(provinceId));
        }

        /// <summary>Returns wards for the given district.</summary>
        [HttpGet("wards")]
        public async Task<IActionResult> GetWards([FromQuery] int districtId)
        {
            if (districtId <= 0)
                return BadRequest("districtId is required.");

            return Ok(await _ghn.GetWardsAsync(districtId));
        }

        /// <summary>Calculates GHN shipping fee for the given destination district/ward.</summary>
        [Authorize]
        [HttpPost("fee")]
        public async Task<IActionResult> CalculateFee([FromBody] ShippingFeeRequestDto dto)
        {
            if (dto.DistrictId <= 0 || string.IsNullOrWhiteSpace(dto.WardCode))
                return BadRequest("districtId and wardCode are required.");

            int weight = dto.Weight > 0 ? dto.Weight : 500;

            _logger.LogInformation(
                "[ShippingFee] Request: districtId={DistrictId}, wardCode={WardCode}, weight={Weight}, insurance={Insurance}",
                dto.DistrictId, dto.WardCode, weight, dto.InsuranceValue);

            try
            {
                int fee = await _ghn.CalculateShippingFeeAsync(dto.DistrictId, dto.WardCode, weight, dto.InsuranceValue);
                _logger.LogInformation("[ShippingFee] GHN returned fee={Fee}", fee);
                return Ok(new ShippingFeeResponseDto { Fee = fee });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ShippingFee] GHN call failed for districtId={DistrictId}, wardCode={WardCode}", dto.DistrictId, dto.WardCode);
                return StatusCode(502, new { error = $"GHN fee calculation failed: {ex.Message}" });
            }
        }

        /// <summary>
        /// Receives GHN order-status webhooks. No authentication — GHN calls this externally.
        /// Stores the raw GHN status in ShippingStatus and drives internal OrderStatus transitions.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> GhnWebhook([FromBody] GhnWebhookDto payload)
        {
            if (string.IsNullOrEmpty(payload.OrderCode))
                return BadRequest();

            _logger.LogInformation(
                "[GHN Webhook] OrderCode={Code}, Status={Status}, Message={Msg}",
                payload.OrderCode, payload.Status, payload.MessageDisplay);

            var bill = await _orderRepo.GetBillByGhnOrderCodeAsync(payload.OrderCode);
            if (bill == null)
            {
                _logger.LogWarning("[GHN Webhook] No bill found for GHN order code {Code}", payload.OrderCode);
                return Ok(); // Acknowledge — prevents GHN from retrying indefinitely
            }

            // Store the raw GHN status string and human-readable message
            bill.ShippingStatus  = payload.Status;
            bill.TrackingMessage = payload.MessageDisplay;
            bill.UpdatedAt       = DateTime.Now;

            // Drive internal OrderStatus transitions from GHN events
            if (payload.Status == "delivered")
            {
                bill.OrderStatus = "COMPLETED";
                // COD payment is collected on delivery
                if (bill.PaymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase))
                    bill.PaymentStatus = "paid";
            }
            else if (payload.Status is "cancel" or "return" or "returned" or "damage" or "lost")
            {
                bill.OrderStatus = "CANCELLED";
            }

            await _orderRepo.SaveChangesAsync();
            return Ok();
        }
    }
}
