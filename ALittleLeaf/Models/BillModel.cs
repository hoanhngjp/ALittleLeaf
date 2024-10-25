using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class BillModel
    {
        [Key]
        public int BillId { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsConfirmed { get; set; }
        public string ShippingStatus { get; set; }
        public string Note { get; set; }

        // Navigation properties
        public UserModel User { get; set; }
        public AddressListModel Address { get; set; }
        public List<BillDetailModel> BillDetails { get; set; }
    }

}
