namespace ALittleLeaf.Api.DTOs.Shipping
{
    // GHN fee calculation response — data.total is the fee in VND
    public class ShippingFeeData
    {
        public int Total { get; set; }
    }

    public class ShippingFeeRequestDto
    {
        public int    DistrictId     { get; set; }
        public string WardCode       { get; set; } = string.Empty;
        public int    Weight         { get; set; } = 500;
        public int    InsuranceValue { get; set; }
    }

    public class ShippingFeeResponseDto
    {
        public int Fee { get; set; }
    }
}
