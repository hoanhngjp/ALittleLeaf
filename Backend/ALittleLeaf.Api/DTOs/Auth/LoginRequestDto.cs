using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public required string Password { get; set; }
    }
}
