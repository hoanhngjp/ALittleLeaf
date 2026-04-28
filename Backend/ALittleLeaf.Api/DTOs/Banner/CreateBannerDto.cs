using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Banner
{
    /// <summary>Request body for POST /api/admin/banners (multipart/form-data).</summary>
    public class CreateBannerDto
    {
        [Required]
        public IFormFile ImageFile { get; set; } = null!;

        public string? TargetUrl { get; set; }

        public int DisplayOrder { get; set; }
    }
}
