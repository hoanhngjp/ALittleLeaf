using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.Tests.Helpers;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALitlleLeaf.Test.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;
        private readonly PasswordHasher<string> _passwordHasher; // Dùng để tạo dữ liệu giả đã hash

        public AuthServiceTests()
        {
            // 1. Tạo In-Memory Database
            _context = DbContextFactory.Create();

            // 2. Mock HttpContext (để lấy Cookie cho hàm Logout)
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // 3. Mock Configuration (để lấy JWT Secret Key)
            _mockConfiguration = new Mock<IConfiguration>();
            // Giả lập key secret (phải dài >= 16 ký tự để HMACSHA256 không lỗi)
            _mockConfiguration.Setup(c => c["JWT_SECRET"]).Returns("super_secret_key_for_testing_123456789");

            // 4. Khởi tạo Service
            _authService = new AuthService(
                _context,
                _mockHttpContextAccessor.Object,
                _mockConfiguration.Object
            );

            // Helper hash password để tạo dữ liệu test
            _passwordHasher = new PasswordHasher<string>();
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(_context);
        }

        #region LOGIN TESTS (Dựa trên Sheet AccountController)

        // LOGIN-04: Kiểm tra đăng nhập thành công
        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            string rawPassword = "Password123!";
            // Tạo user giả với mật khẩu đã hash
            var user = new User
            {
                UserEmail = "valid@test.com",
                UserPassword = _passwordHasher.HashPassword(null, rawPassword), // Hash thật
                UserFullname = "Valid User",
                UserIsActive = true,
                UserRole = "customer"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginModel = new UserLoggedViewModel { UserEmail = "valid@test.com", UserPassword = rawPassword };

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("valid@test.com", result.User.UserEmail);
        }

        // LOGIN-03: Kiểm tra đăng nhập với email không đúng
        [Fact]
        public async Task Login_EmailNotFound_ReturnsErrorMessage()
        {
            // Act
            var loginModel = new UserLoggedViewModel { UserEmail = "notfound@test.com", UserPassword = "123" };
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.False(result.Succeeded);
            // So khớp đúng text trong file Excel: "Tên đăng nhập hoặc mật khẩu bằng nhập không kết nối..."
            Assert.Contains("không kết nối đến tài khoản nào", result.ErrorMessage);
        }

        // LOGIN-02: Kiểm tra đăng nhập với mật khẩu không đúng
        [Fact]
        public async Task Login_WrongPassword_ReturnsErrorMessage()
        {
            // Arrange
            var user = new User
            {
                UserEmail = "wrongpass@test.com",
                UserPassword = _passwordHasher.HashPassword(null, "CorrectPass"),
                UserIsActive = true,
                // --- BỔ SUNG CÁC TRƯỜNG THIẾU ---
                UserFullname = "Wrong Pass User",
                UserRole = "customer",
                UserSex = true,
                UserBirthday = new DateOnly(2000, 1, 1)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginModel = new UserLoggedViewModel { UserEmail = "wrongpass@test.com", UserPassword = "WrongPass" };

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Mật khẩu không chính xác", result.ErrorMessage);
        }

        // LOGIN-01: Kiểm tra đăng nhập với tài khoản bị khóa
        [Fact]
        public async Task Login_LockedAccount_ReturnsLockedMessage()
        {
            // Arrange
            var user = new User
            {
                UserEmail = "locked@test.com",
                UserPassword = _passwordHasher.HashPassword(null, "Pass"),
                UserIsActive = false, // Bị khóa
                // --- BỔ SUNG CÁC TRƯỜNG THIẾU ---
                UserFullname = "Locked User",
                UserRole = "customer",
                UserSex = true,
                UserBirthday = new DateOnly(2000, 1, 1)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginModel = new UserLoggedViewModel { UserEmail = "locked@test.com", UserPassword = "Pass" };

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Tài khoản của bạn đã bị khóa", result.ErrorMessage);
        }

        #endregion

        #region REGISTER TESTS

        // REGISTER-01: Người dùng nhập thông tin đăng kí hợp lệ
        [Fact]
        public async Task Register_ValidData_CreatesUserAndAddress()
        {
            // Arrange
            var registerModel = new RegisterViewModel
            {
                UserEmail = "newuser@test.com",
                UserPassword = "Password123!",
                UserFullname = "New User",
                UserSex = true,
                UserBirthday = new DateOnly(2000, 1, 1),
                Address = "123 Test Street"
            };

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.True(result.Succeeded);

            // 1. Kiểm tra User được lưu vào DB
            var dbUser = _context.Users.FirstOrDefault(u => u.UserEmail == "newuser@test.com");
            Assert.NotNull(dbUser);
            Assert.NotEqual("Password123!", dbUser.UserPassword); // Phải được hash, không lưu plain text

            // 2. Kiểm tra AddressList được tạo tự động (Logic Transaction)
            var dbAddress = _context.AddressLists.FirstOrDefault(a => a.IdUser == dbUser.UserId);
            Assert.NotNull(dbAddress);
            Assert.Equal("123 Test Street", dbAddress.AdrsAddress);
        }

        // REGISTER-05: Đăng ký không thành công vì email đã tồn tại
        [Fact]
        public async Task Register_ExistingEmail_ReturnsErrorMessage()
        {
            // Arrange
            _context.Users.Add(new User
            {
                UserEmail = "exists@test.com",
                // --- BỔ SUNG CÁC TRƯỜNG THIẾU ---
                UserPassword = "hashed_pass", // Bắt buộc phải có
                UserFullname = "Existing User",
                UserRole = "customer",
                UserSex = true,
                UserBirthday = new DateOnly(1990, 1, 1),
                UserIsActive = true
            });
            await _context.SaveChangesAsync();

            var registerModel = new RegisterViewModel { UserEmail = "exists@test.com", UserPassword = "Pass" };

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email đã tồn tại.", result.ErrorMessage);
        }

        #endregion

        #region LOGOUT TESTS

        // LOGOUT-01 & 02: Đăng xuất với người dùng đã đăng nhập (Token hợp lệ)
        [Fact]
        public async Task Logout_ValidToken_RevokesTokenInDb()
        {
            // Arrange
            string validRefreshToken = "valid-refresh-token";

            // 1. Tạo dữ liệu Token trong DB
            var tokenEntity = new RefreshToken
            {
                Token = validRefreshToken,
                JwtId = "jwt-id",
                IsRevoked = false,
                IsUsed = false,
                UserId = 1,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };
            _context.RefreshTokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            // 2. Mock HttpContext để trả về Cookie chứa Token này
            var mockRequest = new Mock<HttpRequest>();
            var mockCookies = new Mock<IRequestCookieCollection>();

            mockCookies.Setup(c => c["X-Refresh-Token"]).Returns(validRefreshToken);
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            // Act
            var result = await _authService.LogoutAsync();

            // Assert
            Assert.True(result);

            // Kiểm tra DB xem token đã bị đánh dấu Revoked chưa
            var dbToken = _context.RefreshTokens.First(t => t.Token == validRefreshToken);
            Assert.True(dbToken.IsRevoked);
            Assert.True(dbToken.IsUsed);
        }

        // LOGOUT-03: Đăng xuất khi chưa đăng nhập (Cookie rỗng)
        [Fact]
        public async Task Logout_NoCookie_ReturnsTrue()
        {
            // Arrange
            var mockRequest = new Mock<HttpRequest>();
            var mockCookies = new Mock<IRequestCookieCollection>();

            mockCookies.Setup(c => c["X-Refresh-Token"]).Returns((string)null); // Không có cookie
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            // Act
            var result = await _authService.LogoutAsync();

            // Assert
            Assert.True(result); // Vẫn trả về true để Controller tiếp tục xóa cookie client cho sạch
        }

        // LOGOUT-04: Đăng xuất với refresh token không tồn tại trong DB (Fake token)
        [Fact]
        public async Task Logout_InvalidToken_ReturnsTrue()
        {
            // Arrange
            // DB trống không, không có token nào

            // Mock Cookie gửi lên token linh tinh
            var mockRequest = new Mock<HttpRequest>();
            var mockCookies = new Mock<IRequestCookieCollection>();

            mockCookies.Setup(c => c["X-Refresh-Token"]).Returns("fake-token");
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            // Act
            var result = await _authService.LogoutAsync();

            // Assert
            Assert.True(result); // Service không crash, trả về true
        }

        #endregion
    }
}
