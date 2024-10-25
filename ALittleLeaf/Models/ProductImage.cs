using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class ProductImage
    {
        [Key]
        public int ImgId { get; set; }
        public int ProductId { get; set; }
        public string ImgName { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public ProductModel Product { get; set; }
    }

}
