namespace ALittleLeaf.Api.DTOs.Order
{
    public class AddressDto
    {
        public int    AdrsId       { get; set; }
        public string AdrsFullname { get; set; } = string.Empty;
        public string AdrsAddress  { get; set; } = string.Empty;
        public string AdrsPhone    { get; set; } = string.Empty;
        public bool   AdrsIsDefault { get; set; }

        // GHN structured address fields
        public int?    ProvinceId   { get; set; }
        public string? ProvinceName { get; set; }
        public int?    DistrictId   { get; set; }
        public string? DistrictName { get; set; }
        public string? WardCode     { get; set; }
        public string? WardName     { get; set; }
    }
}
