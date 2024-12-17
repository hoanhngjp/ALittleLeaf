namespace ALittleLeaf.ViewModels
{
    public class ProductDetailViewModel
    {
        public int ProductId { get; set; }
        public int IdCategory { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsOnSale { get; set; }
        public string[] ProductImages { get; set; }
        public string? PrimaryImage { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
