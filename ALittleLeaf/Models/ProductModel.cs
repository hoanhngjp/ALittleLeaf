using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }

        public int ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public CategoryModel Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<BillDetailModel> BillDetails { get; set; }
    }
}
