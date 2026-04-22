using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [RegularExpression(
            @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
            ErrorMessage = "Email không đúng định dạng.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public required string FullName { get; set; }

        /// <summary>true = Nam, false = Nữ</summary>
        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public bool Sex { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        public DateOnly Birthday { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ của bạn.")]
        public required string Address { get; set; }
    }
}
