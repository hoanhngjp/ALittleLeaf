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
            {
                return new AuthServiceResult 
                { 
                    Succeeded = false,
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu bằng nhập không kết nối đến tài khoản nào. Tìm tài khoản của bạn và đăng nhập."
                };
            }

            var result = _passwordHasher.VerifyHashedPassword(null, user.UserPassword, model.UserPassword);

            if (result != PasswordVerificationResult.Success)
            {
                return new AuthServiceResult 
                { 
                    Succeeded = false,
                    ErrorMessage = "Mật khẩu không chính xác" 
                };

            }

            if (!user.UserIsActive)
            {
                return new AuthServiceResult
                {
                    Succeeded = false,
                    ErrorMessage = "Tài khoản của bạn đã bị khóa"
                };
            }

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

        public async Task<bool> LogoutAsync()
        {
            // 1. Lấy token từ Cookie hiện tại thông qua HttpContextAccessor
            // (Vì Token nằm trong Cookie "X-Refresh-Token" mà ta đã lưu lúc Login)
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                // LOGOUT-003, 004: Không có token hoặc token rỗng -> Coi như xong việc (để Controller xóa cookie)
                return true;
            }

            // 2. Tìm token trong DB
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

            // LOGOUT-004: Token không hợp lệ (không tìm thấy trong DB)
            if (storedToken == null)
            {
                // Không làm gì cả, trả về true để Controller tiếp tục xóa Cookie
                return true;
            }

            // LOGOUT-001, 002: Tìm thấy token (dù hết hạn hay chưa) -> Revoke nó
            try
            {
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // LOGOUT-005: Lỗi DB -> Vẫn trả về true (hoặc false tùy logic log) 
                // để bên ngoài vẫn xóa được Cookie cho người dùng
                return false;
            }
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
