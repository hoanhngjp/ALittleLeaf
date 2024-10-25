using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserFullname { get; set; }
        public bool UserSex { get; set; } // Sử dụng kiểu bool để lưu trữ giới tính (Nam/Nữ)
        public DateTime UserBirthday { get; set; }
        public bool UserIsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserRole { get; set; }

        // Navigation properties
        public List<BillModel> Bills { get; set; }
        public List<AddressListModel> AddressLists { get; set; }
    }
}
