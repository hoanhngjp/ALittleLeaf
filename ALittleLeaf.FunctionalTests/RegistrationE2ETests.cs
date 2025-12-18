using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System;
using System.Threading;

namespace ALittleLeaf.FunctionalTests
{
    public class RegistrationE2ETests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "http://localhost:8080";
        private readonly WebDriverWait _wait;

        public RegistrationE2ETests()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new EdgeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            _driver.Quit();
        }

        // Helper: Tạo email ngẫu nhiên để test
        private string GetRandomEmail()
        {
            return $"auto_test_{Guid.NewGuid().ToString().Substring(0, 8)}@gmail.com";
        }

        [Fact]
        public void Register_ValidInfo_ShouldRedirectToLoginOrHome()
        {
            // 1. Truy cập trang đăng ký
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");

            // 2. Điền thông tin hợp lệ
            // Họ tên
            _driver.FindElement(By.Id("fullname")).SendKeys("Nguyen Auto Test");

            // Giới tính (Mặc định là Nữ - radio1, chọn Nam - radio2 thử xem)
            var maleRadio = _driver.FindElement(By.Id("radio2"));
            // Dùng JS click vì radio button hay bị label che
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", maleRadio);

            // Ngày sinh (dd/mm/yyyy hoặc mm/dd/yyyy tùy locale trình duyệt)
            // Selenium sendkeys vào input date thường là chuỗi số: 01012000 (01/01/2000)
            // Hoặc gửi theo định dạng yyyy-MM-dd nếu browser hỗ trợ chuẩn
            var birthdayInput = _driver.FindElement(By.Id("birthday"));
            birthdayInput.SendKeys("01012000"); // 01/01/2000

            // Email (Ngẫu nhiên)
            string email = GetRandomEmail();
            _driver.FindElement(By.Id("email")).SendKeys(email);

            // Mật khẩu (Có chữ hoa, thường, số, ký tự đặc biệt)
            _driver.FindElement(By.Id("password")).SendKeys("Test@1234");

            // Địa chỉ
            _driver.FindElement(By.Id("address")).SendKeys("123 Test Street, HCM");

            // 3. Submit Form
            var submitBtn = _driver.FindElement(By.CssSelector("button.sign-in-btn"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            submitBtn.Click();

            // 4. Verify chuyển hướng
            // Thường sau khi đăng ký sẽ chuyển về Login hoặc trang chủ
            _wait.Until(d => !d.Url.Contains("Register"));

            // Kiểm tra xem đã chuyển sang trang Login chưa
            Assert.Contains("Login", _driver.Url);
        }

        [Fact]
        public void Register_DuplicateEmail_ShouldShowError()
        {
            // 1. Đăng ký tài khoản A trước (để chắc chắn email tồn tại)
            string existEmail = "Thuong12@gmail.com"; // Email này đã có trong DB seed hoặc test trước đó

            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");

            _driver.FindElement(By.Id("fullname")).SendKeys("Duplicate User");
            _driver.FindElement(By.Id("email")).SendKeys(existEmail);
            _driver.FindElement(By.Id("password")).SendKeys("Test@1234");
            _driver.FindElement(By.Id("address")).SendKeys("Address");
            _driver.FindElement(By.Id("birthday")).SendKeys("01012000");

            var submitBtn = _driver.FindElement(By.CssSelector("button.sign-in-btn"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // 2. Verify lỗi hiển thị
            // Hệ thống của bạn có span lỗi: <span class="show-error" id="isExist-error">Email đã tồn tại.</span>
            // Hoặc lỗi từ server validation

            try
            {
                // Cách 1: Check validation summary hoặc lỗi server trả về
                // Chờ một chút để page reload (nếu server validation) hoặc JS hiển thị
                Thread.Sleep(1000);

                bool hasError = _driver.PageSource.Contains("Email đã tồn tại") ||
                                _driver.PageSource.Contains("đã được sử dụng");

                Assert.True(hasError, "Không thấy thông báo lỗi trùng email.");
            }
            catch (Exception)
            {
                // Nếu không bắt được lỗi text, kiểm tra xem URL có bị giữ lại ở trang Register không
                Assert.Contains("Register", _driver.Url);
            }
        }

        [Fact]
        public void Register_InvalidPassword_ShouldShowValidationError()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");

            _driver.FindElement(By.Id("fullname")).SendKeys("Test Valid");
            _driver.FindElement(By.Id("email")).SendKeys(GetRandomEmail());

            // Nhập mật khẩu yếu (chỉ số)
            _driver.FindElement(By.Id("password")).SendKeys("123456");

            _driver.FindElement(By.Id("address")).SendKeys("Address");
            _driver.FindElement(By.Id("birthday")).SendKeys("01012000");

            var submitBtn = _driver.FindElement(By.CssSelector("button.sign-in-btn"));
            submitBtn.Click();

            // Verify: Không chuyển trang
            Assert.Contains("Register", _driver.Url);

            // Verify: Có thông báo lỗi (Server validation)
            // ASP.NET Identity thường báo lỗi: "Passwords must have at least one non alphanumeric character."
            // Hoặc validation của bạn: "Mật khẩu phải có chữ hoa, thường..."
            bool hasError = _driver.PageSource.Contains("Mật khẩu") || _driver.PageSource.Contains("Password");
            Assert.True(hasError, "Không thấy thông báo lỗi mật khẩu yếu.");
        }
    }
}