namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>Order summary for the admin order list.</summary>
    public class AdminOrderDto
    {
        public int     BillId          { get; set; }
        public long    UserId          { get; set; }
        public string  CustomerName    { get; set; } = string.Empty;
        public string  CustomerEmail   { get; set; } = string.Empty;
        public DateOnly DateCreated    { get; set; }
        public int     TotalAmount     { get; set; }
        public int     ShippingFee     { get; set; }
        public string  PaymentMethod   { get; set; } = string.Empty;
        public string  PaymentStatus   { get; set; } = string.Empty;
        public bool    IsConfirmed      { get; set; }
        public string  OrderStatus      { get; set; } = string.Empty;
        public string  ShippingStatus   { get; set; } = string.Empty;
        public string? TrackingMessage  { get; set; }
        public string? Note             { get; set; }
        public string? GhnOrderCode     { get; set; }
        public int     ItemCount        { get; set; }
    }

    /// <summary>Full order detail including line items for the admin order detail view.</summary>
    public class AdminOrderDetailDto : AdminOrderDto
    {
        public string  RecipientName    { get; set; } = string.Empty;
        public string  RecipientPhone   { get; set; } = string.Empty;
        public string  StreetAddress    { get; set; } = string.Empty;
        public string? WardName         { get; set; }
        public string? DistrictName     { get; set; }
        public string? ProvinceName     { get; set; }
        public IEnumerable<AdminOrderLineItemDto> Items { get; set; } = [];
    }

    public class AdminOrderLineItemDto
    {
        public int    BillDetailId { get; set; }
        public int    ProductId    { get; set; }
        public string ProductName  { get; set; } = string.Empty;
        public string? ProductImg  { get; set; }
        public int    Quantity     { get; set; }
        public int    UnitPrice    { get; set; }
        public int    TotalPrice   { get; set; }
    }

    /// <summary>Payload for PATCH /api/admin/orders/{id}/status</summary>
    public class UpdateOrderStatusDto
    {
        public string? OrderStatus    { get; set; }
        public string? ShippingStatus { get; set; }
        public string? PaymentStatus  { get; set; }
        public bool?   IsConfirmed    { get; set; }
    }
}
