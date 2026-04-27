using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.DTOs.Auth;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Services.Auth;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ALittleLeaf.Tests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IConfiguration>    _mockConfig;
        private readonly AuthService             _authService;
        private readonly PasswordHasher<string>  _hasher;

        public AuthServiceTests()
        {
            _context = DbContextFactory.Create();

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["JWT_SECRET"]).Returns("super_secret_key_for_testing_123456789");
            _mockConfig.Setup(c => c["Jwt:Secret"]).Returns("super_secret_key_for_testing_123456789");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authService = new AuthService(_context, _mockConfig.Object);
            _hasher      = new PasswordHasher<string>();
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        // ── Login ─────────────────────────────────────────────────────────────

        // LOGIN-04: Đăng nhập thành công
        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            string rawPassword = "Password123!";
            _context.Users.Add(new User
            {
                UserEmail    = "valid@test.com",
                UserPassword = _hasher.HashPassword(null!, rawPassword),
                UserFullname = "Valid User",
                UserIsActive = true,
                UserRole     = "customer",
                UserSex      = true,
                UserBirthday = new DateOnly(2000, 1, 1)
            });
            await _context.SaveChangesAsync();

            var dto = new LoginRequestDto { Email = "valid@test.com", Password = rawPassword };

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("valid@test.com", result.User!.UserEmail);
        }

        // LOGIN-03: Email không tồn tại
        [Fact]
        public async Task Login_EmailNotFound_ReturnsError()
        {
            var dto = new LoginRequestDto { Email = "notfound@test.com", Password = "123" };
            var result = await _authService.LoginAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("không kết nối đến tài khoản nào", result.ErrorMessage);
        }

        // LOGIN-02: Mật khẩu sai
        [Fact]
        public async Task Login_WrongPassword_ReturnsError()
        {
            _context.Users.Add(new User
            {
                UserEmail    = "wrongpass@test.com",
                UserPassword = _hasher.HashPassword(null!, "CorrectPass"),
                UserFullname = "Test",
                UserIsActive = true,
                UserRole     = "customer",
                UserSex      = true,
                UserBirthday = new DateOnly(2000, 1, 1)
            });
            await _context.SaveChangesAsync();

            var dto    = new LoginRequestDto { Email = "wrongpass@test.com", Password = "WrongPass" };
            var result = await _authService.LoginAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Equal("Mật khẩu không chính xác.", result.ErrorMessage);
        }

        // LOGIN-01: Tài khoản bị khóa
        [Fact]
        public async Task Login_LockedAccount_ReturnsError()
        {
            _context.Users.Add(new User
            {
                UserEmail    = "locked2@test.com",
                UserPassword = _hasher.HashPassword(null!, "Pass123!"),
                UserFullname = "Locked",
                UserIsActive = false,
                UserRole     = "customer",
                UserSex      = true,
                UserBirthday = new DateOnly(2000, 1, 1)
            });
            await _context.SaveChangesAsync();

            var dto    = new LoginRequestDto { Email = "locked2@test.com", Password = "Pass123!" };
            var result = await _authService.LoginAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Equal("Tài khoản của bạn đã bị khóa.", result.ErrorMessage);
        }

        // ── Register ──────────────────────────────────────────────────────────

        // REGISTER-01: Đăng ký hợp lệ
        [Fact]
        public async Task Register_ValidData_CreatesUser()
        {
            var dto = new RegisterRequestDto
            {
                Email    = "newuser@test.com",
                Password = "Password123!",
                FullName = "New User",
                Sex      = true,
                Birthday = new DateOnly(2000, 1, 1),
            };

            var result = await _authService.RegisterAsync(dto);

            Assert.True(result.Succeeded);

            var dbUser = _context.Users.FirstOrDefault(u => u.UserEmail == "newuser@test.com");
            Assert.NotNull(dbUser);
            Assert.NotEqual("Password123!", dbUser.UserPassword); // must be hashed

            // Address is no longer created at registration — users add addresses from their profile
            var dbAddress = _context.AddressLists.FirstOrDefault(a => a.IdUser == dbUser.UserId);
            Assert.Null(dbAddress);
        }

        // REGISTER-05: Email đã tồn tại
        [Fact]
        public async Task Register_ExistingEmail_ReturnsError()
        {
            _context.Users.Add(new User
            {
                UserEmail    = "exists@test.com",
                UserPassword = "hashed",
                UserFullname = "Existing",
                UserRole     = "customer",
                UserSex      = true,
                UserBirthday = new DateOnly(1990, 1, 1),
                UserIsActive = true
            });
            await _context.SaveChangesAsync();

            var dto    = new RegisterRequestDto { Email = "exists@test.com", Password = "Abc1@xyz", FullName = "X" };
            var result = await _authService.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Equal("Email đã tồn tại.", result.ErrorMessage);
        }

        // ── Logout ────────────────────────────────────────────────────────────

        // LOGOUT-01/02: Token hợp lệ → bị thu hồi trong DB
        [Fact]
        public async Task Logout_ValidToken_RevokesTokenInDb()
        {
            const string token = "valid-refresh-token";
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token      = token,
                JwtId      = "jwt-id",
                IsRevoked  = false,
                IsUsed     = false,
                UserId     = 1,
                AddedDate  = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            });
            await _context.SaveChangesAsync();

            var result = await _authService.LogoutAsync(token);

            Assert.True(result);
            var dbToken = _context.RefreshTokens.First(t => t.Token == token);
            Assert.True(dbToken.IsRevoked);
            Assert.True(dbToken.IsUsed);
        }

        // LOGOUT-03: Không có token (chuỗi rỗng) → trả về true bình thường
        [Fact]
        public async Task Logout_EmptyToken_ReturnsTrue()
        {
            var result = await _authService.LogoutAsync(string.Empty);
            Assert.True(result);
        }

        // LOGOUT-04: Token không tồn tại trong DB → trả về true
        [Fact]
        public async Task Logout_UnknownToken_ReturnsTrue()
        {
            var result = await _authService.LogoutAsync("fake-token-xyz");
            Assert.True(result);
        }
    }
}
