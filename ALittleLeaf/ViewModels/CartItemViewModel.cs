namespace ALittleLeaf.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int IdCategory { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public int ProductPrice { get; set; }
        public string? ProductImg { get; set; } // Thêm thuộc tính cho hình ảnh
        public int TotalPrice => ProductPrice * Quantity;
    }
}
