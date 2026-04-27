using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Order
{
    public class CreateAddressDto
    {
        [Required] [MaxLength(255)] public string AdrsFullname  { get; set; } = string.Empty;
        [Required] [MaxLength(255)] public string AdrsAddress   { get; set; } = string.Empty;
        [Required] [MaxLength(10)]  public string AdrsPhone     { get; set; } = string.Empty;
        public bool AdrsIsDefault { get; set; }

        // GHN structured address fields — optional until Phase 3 enforces them via the UI
        public int?    ProvinceId { get; set; }
        public int?    DistrictId { get; set; }
        public string? WardCode   { get; set; }
    }
}
