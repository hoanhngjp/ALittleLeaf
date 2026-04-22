using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public required string AccessToken { get; set; }

        [Required]
        public required string RefreshToken { get; set; }
    }
}
