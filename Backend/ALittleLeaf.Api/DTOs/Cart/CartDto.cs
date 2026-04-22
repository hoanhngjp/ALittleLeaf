namespace ALittleLeaf.Api.DTOs.Cart
{
    /// <summary>Full cart response returned to the React SPA.</summary>
    public class CartDto
    {
        public int                    CartId     { get; set; }
        public IEnumerable<CartItemDto> Items    { get; set; } = [];
        public int                    TotalItems => Items.Sum(i => i.Quantity);
        public int                    GrandTotal => Items.Sum(i => i.LineTotal);
    }
}
