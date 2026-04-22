namespace ALittleLeaf.Api.DTOs.Order
{
    public class AddressDto
    {
        public int    AdrsId       { get; set; }
        public string AdrsFullname { get; set; } = string.Empty;
        public string AdrsAddress  { get; set; } = string.Empty;
        public string AdrsPhone    { get; set; } = string.Empty;
        public bool   AdrsIsDefault { get; set; }
    }
}
