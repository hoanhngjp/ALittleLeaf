using ALittleLeaf.Api.Controllers;
using ALittleLeaf.Api.DTOs.Auth;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Services.Auth;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ALittleLeaf.Tests.Controllers
{
    /// <summary>
    /// Admin login tests against the shared AuthController.
    /// The "admin portal" is just the same POST /api/auth/login endpoint —
    /// the frontend routes based on the returned role claim.
    /// </summary>
    public class AdminAccountControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController     _controller;

        public AdminAccountControllerTests()
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

        // LOGIN-05: Đăng nhập thành công với quyền Admin → 200 OK + role="admin" trong token
        [Fact]
        public async Task Login_AdminRole_Returns200WithAdminRole()
        {
            var dto = new LoginRequestDto { Email = "admin@test.com", Password = "Pass" };
            var authResult = new AuthServiceResult
            {
                Succeeded    = true,
                AccessToken  = "admin-jwt",
                RefreshToken = "admin-refresh",
                User         = new User
                {
                    UserId       = 1,
                    UserEmail    = "admin@test.com",
                    UserFullname = "Administrator",
                    UserRole     = "admin"
                }
            };

            _mockAuthService.Setup(s => s.LoginAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Login(dto);

            var response = ApiAssert.OkValue<LoginResponseDto>(result);
            Assert.Equal("admin", response.User.Role);
            Assert.Equal("admin-jwt", response.AccessToken);
            Assert.NotNull(response.RefreshToken);
        }

        // Đăng nhập đúng pass nhưng là customer (không phải admin)
        // → API vẫn trả 200; việc phân quyền do [Authorize(Roles="admin")] trên từng admin endpoint
        [Fact]
        public async Task Login_CustomerRole_Returns200ButRoleIsCustomer()
        {
            var dto = new LoginRequestDto { Email = "user@test.com", Password = "Pass" };
            var authResult = new AuthServiceResult
            {
                Succeeded    = true,
                AccessToken  = "customer-jwt",
                RefreshToken = "customer-refresh",
                User         = new User
                {
                    UserId       = 2,
                    UserEmail    = "user@test.com",
                    UserFullname = "Customer",
                    UserRole     = "customer"
                }
            };

            _mockAuthService.Setup(s => s.LoginAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Login(dto);

            var response = ApiAssert.OkValue<LoginResponseDto>(result);
            // Frontend is responsible for gating the admin UI based on this role value
            Assert.Equal("customer", response.User.Role);
        }

        // Đăng nhập sai mật khẩu → 401 Unauthorized
        [Fact]
        public async Task Login_WrongPassword_Returns401()
        {
            var dto = new LoginRequestDto { Email = "admin@test.com", Password = "wrong" };
            _mockAuthService.Setup(s => s.LoginAsync(dto))
                .ReturnsAsync(new AuthServiceResult { Succeeded = false, ErrorMessage = "Mật khẩu không chính xác." });

            var result = await _controller.Login(dto);

            ApiAssert.IsUnauthorized(result);
        }
    }
}
