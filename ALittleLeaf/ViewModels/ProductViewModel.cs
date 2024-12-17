namespace ALittleLeaf.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public int IdCategory { get; set; }
        public string ProductName { get; set; } = null!;
        public int ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public string? ProductImg { get; set; } // Thêm thuộc tính cho hình ảnh
    }
}
