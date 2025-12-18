using ALittleLeaf.Controllers;
using ALittleLeaf.Models;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.Services.Order;
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
    public class CustomerAccountControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IResponseCookies> _mockCookies;
        private readonly Mock<ISession> _mockSession;
        private readonly AccountController _controller;

        public CustomerAccountControllerTests()
        {
            // 1. Setup Service Mocks
            _mockAuthService = new Mock<IAuthService>();
            _mockOrderService = new Mock<IOrderService>();

            // 2. Setup HttpContext Mocks (Cookie & Session)
            _mockCookies = new Mock<IResponseCookies>();
            _mockSession = new Mock<ISession>();

            var mockHttpContext = new Mock<HttpContext>();
            var mockResponse = new Mock<HttpResponse>();

            mockResponse.Setup(r => r.Cookies).Returns(_mockCookies.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
            mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

            // 3. Khởi tạo Controller
            _controller = new AccountController(_mockAuthService.Object, _mockOrderService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                // Setup TempData để test thông báo thành công
                TempData = new TempDataDictionary(mockHttpContext.Object, Mock.Of<ITempDataProvider>())
            };
        }

        #region LOGIN TESTS

        // Test Case: LOGIN-004 (Thành công)
        [Fact]
        public async Task Login_Success_SetsCookieAndRedirects()
        {
            // Arrange
            var model = new UserLoggedViewModel { UserEmail = "user@test.com", UserPassword = "Pass" };
            var authResult = new AuthServiceResult
            {
                Succeeded = true,
                AccessToken = "fake-jwt-token",
                User = new User { UserRole = "customer" }
            };

            _mockAuthService.Setup(s => s.LoginAsync(model)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            // 1. Kiểm tra Redirect về Home
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            // 2. Kiểm tra Cookie AccessToken được lưu
            _mockCookies.Verify(c => c.Append("X-Access-Token", "fake-jwt-token", It.IsAny<CookieOptions>()), Times.Once);

            // 3. Kiểm tra TempData
            Assert.Equal("Đăng nhập thành công", _controller.TempData["SuccessMessage"]);
        }

        // Test Case: LOGIN-001, 002, 003 (Thất bại)
        [Fact]
        public async Task Login_Failure_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var model = new UserLoggedViewModel { UserEmail = "wrong@test.com", UserPassword = "wrong" };
            var authResult = new AuthServiceResult
            {
                Succeeded = false,
                ErrorMessage = "Mật khẩu không chính xác"
            };

            _mockAuthService.Setup(s => s.LoginAsync(model)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Mật khẩu không chính xác", _controller.ViewBag.ErrorMessage);
        }

        #endregion

        #region REGISTER TESTS

        // Test Case: REGISTER-001 (Thành công)
        [Fact]
        public async Task Register_Success_RedirectsToAccountIndex()
        {
            // Arrange
            var model = new RegisterViewModel { UserEmail = "new@test.com", UserPassword = "Pass" };
            var authResult = new AuthServiceResult { Succeeded = true };

            _mockAuthService.Setup(s => s.RegisterAsync(model)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        // Test Case: REGISTER-005 (Thất bại - Email trùng)
        [Fact]
        public async Task Register_Failure_ReturnsViewWithModelError()
        {
            // Arrange
            var model = new RegisterViewModel { UserEmail = "exist@test.com" };
            var authResult = new AuthServiceResult { Succeeded = false, ErrorMessage = "Email đã tồn tại." };

            _mockAuthService.Setup(s => s.RegisterAsync(model)).ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal("Email đã tồn tại.", _controller.ModelState["UserEmail"].Errors[0].ErrorMessage);
        }

        #endregion

        #region LOGOUT TESTS

        // Test Case: LOGOUT-001
        [Fact]
        public async Task Logout_Action_ClearsDataAndRedirectsToLogin()
        {
            // Act
            var result = await _controller.Logout();

            // Assert
            // 1. Verify Service Logout được gọi
            _mockAuthService.Verify(s => s.LogoutAsync(), Times.Once);

            // 2. Verify Cookie bị xóa (Access Token & Refresh Token)
            _mockCookies.Verify(c => c.Delete("X-Access-Token"), Times.Once);
            _mockCookies.Verify(c => c.Delete("X-Refresh-Token"), Times.Once);

            // 3. Verify Session Clear
            _mockSession.Verify(s => s.Clear(), Times.Once);

            // 4. Verify Redirect
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }

        #endregion
    }
}
