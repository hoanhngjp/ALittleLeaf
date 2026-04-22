namespace ALittleLeaf.Api.Models
{
    public class Cart
    {
        public int      CartId    { get; set; }
        public long     UserId    { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual User                User      { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
