using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>Payload for updating an existing product (multipart/form-data).</summary>
    public class UpdateProductDto
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

        public bool IsOnSale { get; set; }

        /// <summary>
        /// When provided, promotes an existing gallery image to primary
        /// (clears IsPrimary on all others). Ignored if NewPrimaryImage is also supplied.
        /// </summary>
        public int? ExistingPrimaryImageId { get; set; }

        /// <summary>
        /// When provided, uploads a new file as the primary image and deletes the old one from Cloudinary.
        /// Takes precedence over ExistingPrimaryImageId.
        /// </summary>
        public IFormFile? NewPrimaryImage { get; set; }

        /// <summary>Additional gallery images to append.</summary>
        public IList<IFormFile>? AdditionalImages { get; set; }

        /// <summary>ImgIds of existing gallery images to remove from Cloudinary.</summary>
        public IList<int>? DeleteImageIds { get; set; }
    }
}
