using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace ALittleLeaf.Test
{
    public class AuthenticationE2ETests
    {

        private IWebDriver CreateDriver()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");

            return new EdgeDriver(options);
        }

        [Fact]
        public void Login_With_Valid_Account_Should_Success()
        {
            // 1. Cấu hình Edge
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");

            IWebDriver driver = new EdgeDriver(options);

            try
            {
                // 2. Truy cập trang Login (SỬA PORT cho đúng)
                driver.Navigate().GoToUrl("http://localhost:8080/Account/Login");

                // 3. Nhập Email
                var emailInput = driver.FindElement(By.Id("email"));
                emailInput.Clear();
                emailInput.SendKeys("Thuong12@gmail.com");

                // 4. Nhập Password
                var passwordInput = driver.FindElement(By.Id("password"));
                passwordInput.Clear();
                passwordInput.SendKeys("Test@123");

                // 5. Click nút đăng nhập
                driver.FindElement(By.CssSelector("button.sign-in-btn")).Click();

                // 6. Chờ redirect sau login
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => !d.Url.Contains("Login"));

                // 7. Assert: đã login thành công
                Assert.DoesNotContain("Login", driver.Url);
            }
            finally
            {
                driver.Quit();
            }
        }
        [Fact]
        public void login_002_AdminLoginSuccess_RedirectToProductManagement()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl("http://localhost:8080/Admin/Account/Login");

                driver.FindElement(By.Id("email"))
                      .SendKeys("hoanhnghiep2704@gmail.com");

                driver.FindElement(By.Id("password"))
                      .SendKeys("4L27hN04Aa@");

                driver.FindElement(By.CssSelector("button.sign-in-btn")).Click();

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Url.Contains("Product") || d.Url.Contains("Admin"));

                // Assert: vào trang quản lý
                Assert.True(
                    driver.Url.Contains("Product") ||
                    driver.Url.Contains("Admin")
                );
            }
            finally
            {
                driver.Quit();
            }
        }
        [Fact]
        public void login_003_WrongPassword_StayOnLoginPage()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl("http://localhost:8080/Account/Login");

                driver.FindElement(By.Id("email"))
                      .SendKeys("Thuong12@gmail.com");

                driver.FindElement(By.Id("password"))
                      .SendKeys("Test@1234");

                driver.FindElement(By.CssSelector("button.sign-in-btn")).Click();

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.Contains("Login"));

                // Assert: vẫn ở login
                Assert.Contains("Login", driver.Url);
            }
            finally
            {
                driver.Quit();
            }
        }
        [Fact]
        public void login_004_EmailNotExist_StayOnLoginPage()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl("http://localhost:8080/Account/Login");

                driver.FindElement(By.Id("email"))
                      .SendKeys("nonexistentuser@gmail.com");

                driver.FindElement(By.Id("password"))
                      .SendKeys("anyPassword");

                driver.FindElement(By.CssSelector("button.sign-in-btn")).Click();

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.Contains("Login"));

                Assert.Contains("Login", driver.Url);
            }
            finally
            {
                driver.Quit();
            }
        }
        [Fact]
        public void login_005_AccountLocked_StayOnLoginPage()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl("http://localhost:8080/Account/Login");

                driver.FindElement(By.Id("email"))
                      .SendKeys("Thuong124@gmail.com");

                driver.FindElement(By.Id("password"))
                      .SendKeys("Test@123");

                driver.FindElement(By.CssSelector("button.sign-in-btn")).Click();

                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.Contains("Login"));

                Assert.Contains("Login", driver.Url);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}

    


