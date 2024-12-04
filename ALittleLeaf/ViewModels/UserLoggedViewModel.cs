using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.ViewModels
{
    public class UserLoggedViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string UserEmail { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string UserPassword { get; set; } = null!;
    }
}
