namespace ALittleLeaf.Api.DTOs.Order
{
    public class OrderDto
    {
        public int      BillId         { get; set; }
        public DateOnly DateCreated    { get; set; }
        public int      TotalAmount    { get; set; }
        public int      ShippingFee    { get; set; }
        public string   PaymentMethod  { get; set; } = string.Empty;
        public string   PaymentStatus  { get; set; } = string.Empty;
        public bool     IsConfirmed      { get; set; }
        public string   OrderStatus      { get; set; } = string.Empty;
        public string   ShippingStatus   { get; set; } = string.Empty;
        public string?  TrackingMessage  { get; set; }
        public string?  Note             { get; set; }
        public string?  GhnOrderCode     { get; set; }
        public AddressDto? ShippingAddress { get; set; }
    }
}
