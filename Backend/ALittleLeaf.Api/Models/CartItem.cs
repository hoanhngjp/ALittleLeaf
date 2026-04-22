namespace ALittleLeaf.Api.Models
{
    public class CartItem
    {
        public int      CartItemId { get; set; }
        public int      CartId     { get; set; }
        public int      ProductId  { get; set; }
        public int      Quantity   { get; set; }
        public DateTime CreatedAt  { get; set; }
        public DateTime UpdatedAt  { get; set; }

        // Navigation properties
        public virtual Cart    Cart    { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
