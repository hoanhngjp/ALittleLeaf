using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Order
{
    public class CreateAddressDto
    {
        [Required] [MaxLength(255)] public string AdrsFullname  { get; set; } = string.Empty;
        [Required] [MaxLength(255)] public string AdrsAddress   { get; set; } = string.Empty;
        [Required] [MaxLength(10)]  public string AdrsPhone     { get; set; } = string.Empty;
        public bool AdrsIsDefault { get; set; }
    }
}
