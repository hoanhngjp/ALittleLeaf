using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ALittleLeaf.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly IConfiguration _configuration; // <-- Inject Configuration

        public AuthService(AlittleLeafDecorContext context,
                           IHttpContextAccessor httpContextAccessor,
                           IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<string>();
        }

        public async Task<AuthServiceResult> LoginAsync(UserLoggedViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.UserEmail);

            if (user == null)
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Thông tin đăng nhập không hợp lệ." };

            var result = _passwordHasher.VerifyHashedPassword(null, user.UserPassword, model.UserPassword);
            if (result != PasswordVerificationResult.Success)
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Thông tin đăng nhập không hợp lệ." };

            if (!user.UserIsActive)
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Tài khoản của bạn đã bị khóa." };

            // --- THAY ĐỔI LỚN: KHÔNG SET SESSION NỮA, MÀ SINH TOKEN ---
            return await GenerateJwtToken(user);
        }

        public async Task<AuthServiceResult> RegisterAsync(RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.UserEmail == model.UserEmail))
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Email đã tồn tại." };

            string hashedPass = _passwordHasher.HashPassword(null, model.UserPassword);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new User
                    {
                        UserEmail = model.UserEmail,
                        UserPassword = hashedPass,
                        UserFullname = model.UserFullname,
                        UserSex = model.UserSex,
                        UserBirthday = model.UserBirthday,
                        UserIsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        UserRole = model.UserRole
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var address = new AddressList
                    {
                        IdUser = user.UserId,
                        AdrsFullname = user.UserFullname,
                        AdrsAddress = model.Address,
                        AdrsPhone = "N/A",
                        AdrsIsDefault = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.AddressLists.Add(address);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // --- THAY ĐỔI: SINH TOKEN SAU KHI ĐĂNG KÝ ---
                    return await GenerateJwtToken(user);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return new AuthServiceResult { Succeeded = false, ErrorMessage = "Lỗi hệ thống." };
                }
            }
        }

        public async Task LogoutAsync()
        {
            // Với JWT, logout phía server chủ yếu là xóa cookie ở Controller.
            // Ở đây ta có thể đánh dấu RefreshToken là revoked (nếu muốn chặt chẽ hơn).
            await Task.CompletedTask;
        }

        // --- HÀM PRIVATE: SINH JWT VÀ REFRESH TOKEN ---
        private async Task<AuthServiceResult> GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            // Lấy SecretKey từ .env (thông qua Configuration)
            var key = Encoding.ASCII.GetBytes(_configuration["JWT_SECRET"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserEmail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("FullName", user.UserFullname),
                    new Claim(ClaimTypes.Role, user.UserRole) // Quan trọng để phân quyền Admin/User
                }),
                // Access Token sống 30 phút
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                // Bạn nên thêm Issuer và Audience vào appsettings.json và đọc ở đây
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            // Tạo Refresh Token
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.UserId,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30), // Refresh Token sống 30 ngày
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthServiceResult
            {
                Succeeded = true,
                User = user,
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
