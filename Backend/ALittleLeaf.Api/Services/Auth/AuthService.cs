using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.DTOs.Auth;
using ALittleLeaf.Api.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ALittleLeaf.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly IGoogleTokenValidator _googleTokenValidator;

        // Access token lifetime in minutes (default 60, configurable via appsettings)
        private int AccessTokenMinutes =>
            int.TryParse(_configuration["Jwt:AccessTokenExpirationMinutes"], out var m) ? m : 60;

        // Refresh token lifetime in days (default 7)
        private int RefreshTokenDays =>
            int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var d) ? d : 7;

        public AuthService(
            AlittleLeafDecorContext context,
            IConfiguration configuration,
            IGoogleTokenValidator googleTokenValidator)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<string>();
            _googleTokenValidator = googleTokenValidator;
        }

        // ── Login ─────────────────────────────────────────────────────────────

        public async Task<AuthServiceResult> LoginAsync(LoginRequestDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail == dto.Email);

            if (user == null)
                return Fail("Tên đăng nhập hoặc mật khẩu không kết nối đến tài khoản nào.");

            if (string.IsNullOrEmpty(user.UserPassword))
                return Fail("Tài khoản này được đăng ký qua Google. Vui lòng đăng nhập bằng Google.");

            var verifyResult = _passwordHasher.VerifyHashedPassword(null!, user.UserPassword, dto.Password);
            if (verifyResult != PasswordVerificationResult.Success)
                return Fail("Mật khẩu không chính xác.");

            if (!user.UserIsActive)
                return Fail("Tài khoản của bạn đã bị khóa.");

            return await IssueTokenPairAsync(user);
        }

        // ── Register ──────────────────────────────────────────────────────────

        public async Task<AuthServiceResult> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.UserEmail == dto.Email))
                return Fail("Email đã tồn tại.");

            var hashedPassword = _passwordHasher.HashPassword(null!, dto.Password);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserEmail    = dto.Email,
                    UserPassword = hashedPassword,
                    UserFullname = dto.FullName,
                    UserSex      = dto.Sex,
                    UserBirthday = dto.Birthday,
                    UserIsActive = true,
                    UserRole     = "customer",   // new registrations are always customers
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await IssueTokenPairAsync(user);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Fail("Lỗi hệ thống. Vui lòng thử lại.");
            }
        }

        // ── Refresh Token ─────────────────────────────────────────────────────

        public async Task<AuthServiceResult> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var jwtSecret = GetJwtSecret();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            // 1. Validate the expired access token (ignore lifetime)
            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(dto.AccessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ValidateIssuer           = true,
                    ValidIssuer              = _configuration["Jwt:Issuer"],
                    ValidateAudience         = true,
                    ValidAudience            = _configuration["Jwt:Audience"],
                    ValidateLifetime         = false  // allow expired tokens for refresh
                }, out _);
            }
            catch
            {
                return Fail("Access token không hợp lệ.");
            }

            // 2. Extract the JTI from the expired access token
            var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
                return Fail("Access token không hợp lệ.");

            // 3. Verify the refresh token in the database
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

            if (storedToken == null)
                return Fail("Refresh token không tồn tại.");

            if (storedToken.IsUsed)
                return Fail("Refresh token đã được sử dụng.");

            if (storedToken.IsRevoked)
                return Fail("Refresh token đã bị thu hồi.");

            if (storedToken.ExpiryDate < DateTime.UtcNow)
                return Fail("Refresh token đã hết hạn.");

            if (storedToken.JwtId != jti)
                return Fail("Refresh token không khớp với access token.");

            // 4. Mark old refresh token as used
            storedToken.IsUsed = true;
            _context.RefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            // 5. Load user and issue a fresh token pair
            var user = await _context.Users.FindAsync(storedToken.UserId);
            if (user == null || !user.UserIsActive)
                return Fail("Tài khoản không tồn tại hoặc đã bị khóa.");

            return await IssueTokenPairAsync(user);
        }

        // ── Logout ────────────────────────────────────────────────────────────

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return true;   // nothing to revoke, treat as success

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null)
                return true;   // token unknown, already gone

            storedToken.IsRevoked = true;
            storedToken.IsUsed    = true;
            _context.RefreshTokens.Update(storedToken);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ── Google SSO ────────────────────────────────────────────────────────

        public async Task<AuthServiceResult> GoogleLoginAsync(string idToken)
        {
            // 1. Verify the ID Token with Google's public keys
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var clientId = _configuration["Google:ClientId"]
                               ?? throw new InvalidOperationException("Google:ClientId is not configured.");
                payload = await _googleTokenValidator.ValidateAsync(idToken, clientId);
            }
            catch (InvalidJwtException ex)
            {
                return Fail($"Google token không hợp lệ: {ex.Message}");
            }

            // 2. Security gate: only accept verified email addresses
            if (payload.EmailVerified != true)
                return Fail("Email của tài khoản Google chưa được xác minh.");

            var email    = payload.Email;
            var googleId = payload.Subject;
            var fullName = payload.Name ?? email;

            // 3. Look up existing user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email);

            if (user == null)
            {
                // Case A: New user — create account linked to Google
                user = new User
                {
                    UserEmail    = email,
                    UserPassword = null,
                    UserFullname = fullName,
                    UserSex      = false,
                    UserBirthday = DateOnly.FromDateTime(DateTime.UtcNow),
                    UserIsActive = true,
                    UserRole     = "customer",
                    AuthProvider = "google",
                    GoogleId     = googleId,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (!user.UserIsActive)
            {
                return Fail("Tài khoản của bạn đã bị khóa.");
            }
            else
            {
                // Case B: Existing Google user OR Case C: Existing local user (account merge)
                // In both cases: update/set the GoogleId and mark provider
                user.GoogleId     = googleId;
                user.AuthProvider ??= "google";   // preserve "local" if already set, otherwise set "google"
                user.UpdatedAt    = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Note: payload.Picture (Google avatar URL) available for future UserAvatarUrl column
                // _logger.LogInformation("Google login for {Email}, avatar: {Picture}", email, payload.Picture);
            }

            return await IssueTokenPairAsync(user);
        }

        // ── Private helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Generates a JWT access token + refresh token, persists the refresh token, and
        /// returns a successful <see cref="AuthServiceResult"/>.
        ///
        /// JWT claims include <see cref="ClaimTypes.NameIdentifier"/> mapped to UserId —
        /// this is required by Phase 5 Cart API which extracts UserId server-side from the token.
        /// </summary>
        private async Task<AuthServiceResult> IssueTokenPairAsync(User user)
        {
            var jwtSecret = GetJwtSecret();
            var key       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var jti       = Guid.NewGuid().ToString();

            var claims = new[]
            {
                // Phase 5 (Cart) and all protected endpoints rely on NameIdentifier for userId.
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserEmail),
                new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim("FullName", user.UserFullname),
                new Claim(ClaimTypes.Role, user.UserRole),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                Issuer             = _configuration["Jwt:Issuer"],
                Audience           = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler      = new JwtSecurityTokenHandler();
            var token        = handler.CreateToken(tokenDescriptor);
            var accessToken  = handler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId      = jti,
                Token      = Guid.NewGuid() + "-" + Guid.NewGuid(),
                UserId     = user.UserId,
                IsUsed     = false,
                IsRevoked  = false,
                AddedDate  = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenDays)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthServiceResult
            {
                Succeeded    = true,
                User         = user,
                AccessToken  = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        private string GetJwtSecret() =>
            Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");

        private static AuthServiceResult Fail(string message) =>
            new() { Succeeded = false, ErrorMessage = message };
    }
}
