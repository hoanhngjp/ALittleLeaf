namespace ALittleLeaf.Api.DTOs.Order
{
    /// <summary>Unified result returned by ConfirmPaymentAsync.</summary>
    public class PaymentResultDto
    {
        public bool   Success        { get; set; }
        public int    BillId         { get; set; }
        public string VnpResponseCode { get; set; } = string.Empty;
        public string Message        { get; set; } = string.Empty;
        /// <summary>True when the order was already confirmed (idempotency guard).</summary>
        public bool   AlreadyPaid    { get; set; }
    }
}
