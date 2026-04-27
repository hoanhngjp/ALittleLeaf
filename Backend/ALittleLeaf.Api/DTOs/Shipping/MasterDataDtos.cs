namespace ALittleLeaf.Api.DTOs.Shipping
{
    // No [JsonPropertyName] here — these are the output DTOs sent to the frontend.
    // GHN's PascalCase keys (e.g. "ProvinceID") are handled by PropertyNameCaseInsensitive
    // in GhnService so ASP.NET Core can serialize these with its default camelCase policy.
    public class ProvinceDto
    {
        public int    ProvinceId   { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
    }

    public class DistrictDto
    {
        public int    DistrictId   { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public int    ProvinceId   { get; set; }
    }

    public class WardDto
    {
        public string WardCode   { get; set; } = string.Empty;
        public string WardName   { get; set; } = string.Empty;
        public int    DistrictId { get; set; }
    }
}
