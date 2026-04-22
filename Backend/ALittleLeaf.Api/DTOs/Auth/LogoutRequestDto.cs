using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Auth
{
    public class LogoutRequestDto
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
