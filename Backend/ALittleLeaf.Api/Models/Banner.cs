namespace ALittleLeaf.Api.Models
{
    public class Banner
    {
        public int BannerId { get; set; }

        public string ImageUrl { get; set; } = null!;

        /// <summary>Cloudinary public_id — required for deletion via the Cloudinary API.</summary>
        public string PublicId { get; set; } = null!;

        /// <summary>Optional click-through URL shown when a visitor clicks the banner.</summary>
        public string? TargetUrl { get; set; }

        /// <summary>Lower value = displayed first in the slider.</summary>
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
