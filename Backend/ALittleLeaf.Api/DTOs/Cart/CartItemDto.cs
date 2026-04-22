namespace ALittleLeaf.Api.DTOs.Cart
{
    /// <summary>A single line item inside the cart response.</summary>
    public class CartItemDto
    {
        public int     CartItemId   { get; set; }
        public int     ProductId    { get; set; }
        public string  ProductName  { get; set; } = string.Empty;
        public int     ProductPrice { get; set; }
        public string? ProductImage { get; set; }
        public int     Quantity     { get; set; }
        public int     LineTotal    => ProductPrice * Quantity;
    }
}
