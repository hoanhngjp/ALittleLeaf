using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Cart
{
    /// <summary>Request body for POST /api/cart/items.</summary>
    public class AddToCartDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive integer.")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity  { get; set; } = 1;
    }
}
