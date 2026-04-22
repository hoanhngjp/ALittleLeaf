using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Order
{
    /// <summary>
    /// Request body for POST /api/orders.
    /// Either supply an existing <see cref="AddressId"/> OR fill in the three
    /// NewXxx fields to create a one-time shipping address.
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>Existing saved address. Leave null to use the inline address fields.</summary>
        public int? AddressId { get; set; }

        /// <summary>Required when <see cref="AddressId"/> is null.</summary>
        [MaxLength(255)] public string? NewFullName { get; set; }
        [MaxLength(255)] public string? NewAddress  { get; set; }
        [MaxLength(10)]  public string? NewPhone    { get; set; }

        /// <summary>"COD" or "VNPAY".</summary>
        [Required]
        public string PaymentMethod { get; set; } = "COD";

        [MaxLength(255)] public string? Note { get; set; }
    }
}
