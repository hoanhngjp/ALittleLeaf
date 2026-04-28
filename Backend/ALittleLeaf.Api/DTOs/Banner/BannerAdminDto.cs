namespace ALittleLeaf.Api.DTOs.Banner
{
    /// <summary>Full banner shape returned by admin GET endpoints (includes management fields).</summary>
    public class BannerAdminDto
    {
        public int      BannerId     { get; set; }
        public string   ImageUrl     { get; set; } = null!;
        public string   PublicId     { get; set; } = null!;
        public string?  TargetUrl    { get; set; }
        public int      DisplayOrder { get; set; }
        public bool     IsActive     { get; set; }
        public DateTime CreatedAt    { get; set; }
        public DateTime UpdatedAt    { get; set; }
    }
}
