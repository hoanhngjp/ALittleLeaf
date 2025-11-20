using ALittleLeaf.Models;

namespace ALittleLeaf.Services.Auth
{
    public class AuthServiceResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
