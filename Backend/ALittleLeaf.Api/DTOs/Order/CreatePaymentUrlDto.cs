using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Order
{
    /// <summary>Request body for POST /api/payment/create-url.</summary>
    public class CreatePaymentUrlDto
    {
        [Required]
        public int BillId { get; set; }
    }
}
