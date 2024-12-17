using Microsoft.AspNetCore.Identity;

namespace ALittleLeaf.Controllers
{
    public class HashPasswordController
    {
        private readonly PasswordHasher<string> _passwordHasher;

        public HashPasswordController()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        // Hash mật khẩu
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        // Verify mật khẩu
        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
