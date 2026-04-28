namespace ALittleLeaf.Api.DTOs.Banner
{
    /// <summary>
    /// Request body for PUT /api/admin/banners/{id} (JSON).
    /// All fields are optional — only non-null values are applied.
    /// </summary>
    public class UpdateBannerDto
    {
        public bool?   IsActive     { get; set; }
        public int?    DisplayOrder { get; set; }
        public string? TargetUrl    { get; set; }
    }
}
