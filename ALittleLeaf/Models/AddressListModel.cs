using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class AddressListModel
    {
        [Key]
        public int AdrsId { get; set; }
        public int UserId { get; set; }
        public string AdrsFullname { get; set; }
        public string AdrsAddress { get; set; }
        public string AdrsPhone { get; set; }
        public bool AdrsIsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public UserModel User { get; set; }
    }

}
