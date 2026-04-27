using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Auth;
using ALittleLeaf.Api.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Tests.Helpers;

namespace ALittleLeaf.Tests.Controllers
{
    public class CustomerAccountControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController     _controller;

        public CustomerAccountControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        // ── Login ─────────────────────────────────────────────────────────────

        // LOGIN-04: Đăng nhập thành công → 200 OK với JWT
        [Fact]
        public async Task Login_Success_Returns200WithTokens()
        {
            var dto = new LoginRequestDto { Email = "user@test.com", Password = "Pass123!" };
            var authResult = new AuthServiceResult
            {
                Succeeded    = true,
                AccessToken  = "fake-jwt",
                RefreshToken = "fake-refresh",
                User         = new User { UserId = 1, UserEmail = "user@test.com", UserFullname = "Test User", UserRole = "customer" }
            };

            _mockAuthService.Setup(s => s.LoginAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Login(dto);

            var response = ApiAssert.OkValue<LoginResponseDto>(result);
            Assert.Equal("fake-jwt", response.AccessToken);
            Assert.NotNull(response.RefreshToken);
            Assert.Equal("user@test.com", response.User.Email);
        }

        // LOGIN-01/02/03: Đăng nhập thất bại → 401 Unauthorized
        [Fact]
        public async Task Login_Failure_Returns401()
        {
            var dto = new LoginRequestDto { Email = "wrong@test.com", Password = "wrong" };
            _mockAuthService.Setup(s => s.LoginAsync(dto))
                .ReturnsAsync(new AuthServiceResult { Succeeded = false, ErrorMessage = "Mật khẩu không chính xác." });

            var result = await _controller.Login(dto);

            ApiAssert.IsUnauthorized(result);
        }

        // ── Register ──────────────────────────────────────────────────────────

        // REGISTER-01: Đăng ký hợp lệ → 201 Created
        [Fact]
        public async Task Register_Success_Returns201()
        {
            var dto = new RegisterRequestDto
            {
                Email    = "new@test.com",
                Password = "Password123!",
                FullName = "New User",
            };
            var authResult = new AuthServiceResult
            {
                Succeeded    = true,
                AccessToken  = "new-jwt",
                RefreshToken = "new-refresh",
                User         = new User { UserId = 5, UserEmail = "new@test.com", UserFullname = "New User", UserRole = "customer" }
            };

            _mockAuthService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Register(dto);

            var created = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        // REGISTER-05: Email đã tồn tại → 409 Conflict
        [Fact]
        public async Task Register_DuplicateEmail_Returns409()
        {
            var dto = new RegisterRequestDto { Email = "exists@test.com", Password = "Abc1@xyz", FullName = "X" };
            _mockAuthService.Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync(new AuthServiceResult { Succeeded = false, ErrorMessage = "Email đã tồn tại." });

            var result = await _controller.Register(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, conflict.StatusCode);
        }

        // ── Logout ────────────────────────────────────────────────────────────

        // LOGOUT-01/02: Đăng xuất với token hợp lệ → 200 OK
        [Fact]
        public async Task Logout_ValidToken_Returns200()
        {
            var dto = new LogoutRequestDto { RefreshToken = "some-refresh-token" };
            _mockAuthService.Setup(s => s.LogoutAsync("some-refresh-token")).ReturnsAsync(true);

            var result = await _controller.Logout(dto);

            ApiAssert.IsOk(result);
            _mockAuthService.Verify(s => s.LogoutAsync("some-refresh-token"), Times.Once);
        }
    }
}
