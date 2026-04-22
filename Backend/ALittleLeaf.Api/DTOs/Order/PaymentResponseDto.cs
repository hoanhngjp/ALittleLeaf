namespace ALittleLeaf.Api.DTOs.Order
{
    /// <summary>Response body for POST /api/payment/create-url.</summary>
    public class PaymentResponseDto
    {
        public int    BillId     { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
