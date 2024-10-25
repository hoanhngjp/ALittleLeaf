using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryParentId { get; set; }
        public string CategoryImg { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public List<ProductModel> Products { get; set; }
    }

}
