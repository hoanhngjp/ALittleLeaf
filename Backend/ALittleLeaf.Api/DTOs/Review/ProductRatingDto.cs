namespace ALittleLeaf.Api.DTOs.Review
{
    public class ProductRatingDto
    {
        public int ProductId { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
