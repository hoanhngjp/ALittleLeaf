using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage="*")]
        public long UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string UserEmail { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public string UserPassword { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string UserFullname { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public bool UserSex { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        [DataType(DataType.Date)]
        public DateOnly UserBirthday { get; set; }

        public bool UserIsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string UserRole { get; set; } = null!;
    }
}
