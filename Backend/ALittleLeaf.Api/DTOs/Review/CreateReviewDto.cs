using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
