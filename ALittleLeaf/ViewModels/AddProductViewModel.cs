using ALittleLeaf.Models;

namespace ALittleLeaf.ViewModels
{
    public class AddProductViewModel
    {
        public int ProductId { get; set; }

        public int IdCategory { get; set; }

        public string ProductName { get; set; } = null!;

        public int ProductPrice { get; set; }

        public string? ProductDescription { get; set; }

        public int QuantityInStock { get; set; }

        public bool IsOnSale { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public List<IFormFile> ProductImages { get; set; } // Lưu ảnh tải lên
    }
}
