using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng.")]
        public string UserEmail { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string UserFullname { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public bool UserSex { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        [DataType(DataType.Date)]
        public DateOnly UserBirthday { get; set; }
        public bool UserIsActive { get; set; }
        public string UserRole { get; set; } = "customer";

        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ của bạn.")]
        public string Address { get; set; }
    }
}
