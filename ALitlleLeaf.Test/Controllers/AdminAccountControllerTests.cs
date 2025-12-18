using ALittleLeaf.Areas.Admin.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALitlleLeaf.Test.Controllers
{
    public class AdminAccountControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IResponseCookies> _mockCookies;
        private readonly Mock<ISession> _mockSession;
        private readonly AccountController _controller; // Admin Controller

        public AdminAccountControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockCookies = new Mock<IResponseCookies>();
            _mockSession = new Mock<ISession>();

            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();

            mockResponse.Setup(r => r.Cookies).Returns(_mockCookies.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
            mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

            _controller = new AccountController(_mockAuthService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                TempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>())
            };
        }

        // LOGIN-005: Đăng nhập thành công với quyền Admin
        [Fact]
        public async Task Login_AdminRole_RedirectsToDashboard()
        {
            // Arrange
            var model = new UserLoggedViewModel { UserEmail = "admin@test.com", UserPassword = "Pass" };

            // Mock kết quả trả về UserRole = "admin"
            var authResult = new AuthServiceResult
            {
                Succeeded = true,
                AccessToken = "admin-token",
                RefreshToken = "refresh-token",
                User = new User { UserRole = "admin", UserEmail = "admin@test.com", UserFullname = "Admin" }
            };

            _mockAuthService.Setup(s => s.LoginAsync(model)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Dashboard", redirectResult.ControllerName);

            // Kiểm tra Cookie Refresh Token được lưu (Đặc thù Admin)
            _mockCookies.Verify(c => c.Append("X-Refresh-Token", "refresh-token", It.IsAny<CookieOptions>()), Times.Once);
        }

        // Test Case: Đăng nhập đúng pass nhưng KHÔNG PHẢI ADMIN (UserRole = "customer")
        [Fact]
        public async Task Login_CustomerRole_ReturnsErrorForAdminPortal()
        {
            // Arrange
            var model = new UserLoggedViewModel { UserEmail = "user@test.com", UserPassword = "Pass" };

            // Mock UserRole = "customer"
            var authResult = new AuthServiceResult
            {
                Succeeded = true,
                User = new User { UserRole = "customer" }
            };

            _mockAuthService.Setup(s => s.LoginAsync(model)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Bạn không có quyền truy cập Admin.", _controller.ViewBag.ErrorMessage);

            // Đảm bảo KHÔNG set cookie nếu sai quyền
            _mockCookies.Verify(c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never);
        }
    }
}
