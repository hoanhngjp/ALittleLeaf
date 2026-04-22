using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Services.Auth
{
    /// <summary>Internal result object passed from AuthService to AuthController.</summary>
    public class AuthServiceResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
