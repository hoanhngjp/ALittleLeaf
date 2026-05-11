namespace ALittleLeaf.Api.DTOs.Review
{
    public class AdminReviewDto
    {
        public int ReviewId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
