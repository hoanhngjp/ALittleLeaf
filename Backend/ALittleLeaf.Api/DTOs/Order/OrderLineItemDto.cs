namespace ALittleLeaf.Api.DTOs.Order
{
    public class OrderLineItemDto
    {
        public int    BillDetailId { get; set; }
        public int    ProductId    { get; set; }
        public string ProductName  { get; set; } = string.Empty;
        public string? ProductImg  { get; set; }
        public int    Quantity     { get; set; }
        public int    UnitPrice    { get; set; }
        public int    TotalPrice   { get; set; }
    }
}
