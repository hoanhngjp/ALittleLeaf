using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class BillDetailModel
    {
        [Key]
        public int BillDetailId { get; set; }
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public BillModel Bill { get; set; }
        public ProductModel Product { get; set; }
    }

}
