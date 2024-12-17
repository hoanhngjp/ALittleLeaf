namespace ALittleLeaf.ViewModels
{
    public class ExistingImageViewModel
    {
        public int ImgId { get; set; }
        public string ImgName { get; set; } 
        public bool IsPrimary { get; set; }
    }

    public class EditProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int IdCategory { get; set; }
        public int ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsOnSale { get; set; }
        public List<IFormFile>? NewProductImages { get; set; }
        public List<ExistingImageViewModel>? ExistingProductImages { get; set; } // Lưu danh sách ảnh đã tồn 
        public int? IsPrimaryImg { get; set; }  // Thêm trường để lưu ID ảnh chính
    }
}
