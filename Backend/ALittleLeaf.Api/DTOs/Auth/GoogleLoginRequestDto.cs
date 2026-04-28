using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Auth
{
    public class GoogleLoginRequestDto
    {
        [Required]
        public required string IdToken { get; set; }
    }
}
