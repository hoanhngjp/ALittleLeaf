using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>Payload for creating a new product (multipart/form-data).</summary>
    public class CreateProductDto
    {
        [Required]
        public int IdCategory { get; set; }

        [Required, MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;

        [Required, Range(1, int.MaxValue)]
        public int ProductPrice { get; set; }

        public string? ProductDescription { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        public bool IsOnSale { get; set; } = true;

        /// <summary>Primary image file. Optional — can be added later.</summary>
        public IFormFile? PrimaryImage { get; set; }

        /// <summary>Additional gallery images.</summary>
        public IList<IFormFile>? AdditionalImages { get; set; }
    }
}
