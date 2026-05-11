namespace ALittleLeaf.Api.DTOs.Review
{
    public class PagedReviewResultDto
    {
        public List<ReviewDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
