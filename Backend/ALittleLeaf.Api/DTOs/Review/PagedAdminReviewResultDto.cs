namespace ALittleLeaf.Api.DTOs.Review
{
    public class PagedAdminReviewResultDto
    {
        public List<AdminReviewDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
