namespace ALittleLeaf.Api.DTOs.Review
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
