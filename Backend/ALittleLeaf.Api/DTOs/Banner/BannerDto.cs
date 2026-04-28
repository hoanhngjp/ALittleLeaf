namespace ALittleLeaf.Api.DTOs.Banner
{
    /// <summary>Public-facing banner shape returned by GET /api/banners.</summary>
    public class BannerDto
    {
        public int     BannerId     { get; set; }
        public string  ImageUrl     { get; set; } = null!;
        public string? TargetUrl    { get; set; }
        public int     DisplayOrder { get; set; }
    }
}
