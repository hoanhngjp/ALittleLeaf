using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Cart
{
    /// <summary>Request body for PUT /api/cart/items/{productId}.</summary>
    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }
    }
}
