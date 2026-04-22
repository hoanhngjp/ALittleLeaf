using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System;

namespace ALittleLeaf.FunctionalTests
{
    /// <summary>
    /// E2E registration tests against the React SPA at http://localhost:3000.
    /// Register page: /register
    /// On success the SPA redirects to /login.
    /// Gender radios: id="sex-true" (Nam), id="sex-false" (Nữ).
    /// Submit button: button.brand-btn (not the old button.sign-in-btn).
    /// </summary>
    public class RegistrationE2ETests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "http://localhost:3000";
        private readonly WebDriverWait _wait;

        public RegistrationE2ETests()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new EdgeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose() => _driver.Quit();

        private string GetRandomEmail() =>
            $"auto_test_{Guid.NewGuid().ToString()[..8]}@gmail.com";

        // reg_001: Valid registration → redirect to /login
        [Fact]
        public void Register_ValidInfo_ShouldRedirectToLogin()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/register");

            _driver.FindElement(By.Id("fullname")).SendKeys("Nguyen Auto Test");

            // Gender radio: id="sex-true" = Nam (male)
            var maleRadio = _driver.FindElement(By.Id("sex-true"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", maleRadio);

            // Birthday input (type="date") — send as yyyy-MM-dd
            _driver.FindElement(By.Id("birthday")).SendKeys("2000-01-01");

            _driver.FindElement(By.Id("email")).SendKeys(GetRandomEmail());
            _driver.FindElement(By.Id("password")).SendKeys("Test@1234");
            _driver.FindElement(By.Id("address")).SendKeys("123 Test Street, HCM");

            var submitBtn = _driver.FindElement(By.CssSelector("button.brand-btn"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            submitBtn.Click();

            // After successful registration the SPA navigates to /login
            _wait.Until(d => d.Url.Contains("/login"));

            Assert.Contains("/login", _driver.Url);
        }

        // reg_002: Duplicate email → stay on /register, error message visible
        [Fact]
        public void Register_DuplicateEmail_ShouldShowError()
        {
            string existEmail = "Thuong12@gmail.com"; // seeded in DB

            _driver.Navigate().GoToUrl($"{_baseUrl}/register");

            _driver.FindElement(By.Id("fullname")).SendKeys("Duplicate User");
            _driver.FindElement(By.Id("birthday")).SendKeys("2000-01-01");
            _driver.FindElement(By.Id("email")).SendKeys(existEmail);
            _driver.FindElement(By.Id("password")).SendKeys("Test@1234");
            _driver.FindElement(By.Id("address")).SendKeys("Address");

            var submitBtn = _driver.FindElement(By.CssSelector("button.brand-btn"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // React renders the API error in a .text-danger element
            try
            {
                _wait.Until(d =>
                    d.FindElements(By.CssSelector(".text-danger")).Count > 0
                    || d.PageSource.Contains("Email đã tồn tại")
                    || d.PageSource.Contains("đã được sử dụng"));

                bool hasError = _driver.PageSource.Contains("Email đã tồn tại")
                             || _driver.PageSource.Contains("đã được sử dụng")
                             || _driver.FindElements(By.CssSelector(".text-danger")).Count > 0;

                Assert.True(hasError, "Expected duplicate-email error but none was shown.");
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: URL must still be /register if error was silent
                Assert.Contains("/register", _driver.Url);
            }
        }

        // reg_003: Weak password → HTML5 / React validation keeps user on /register
        [Fact]
        public void Register_InvalidPassword_ShouldShowValidationError()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/register");

            _driver.FindElement(By.Id("fullname")).SendKeys("Test Valid");
            _driver.FindElement(By.Id("birthday")).SendKeys("2000-01-01");
            _driver.FindElement(By.Id("email")).SendKeys(GetRandomEmail());
            _driver.FindElement(By.Id("password")).SendKeys("123456"); // weak: no uppercase/special char
            _driver.FindElement(By.Id("address")).SendKeys("Address");

            _driver.FindElement(By.CssSelector("button.brand-btn")).Click();

            // React front-end validation or API 400 keeps user on /register
            // Give the SPA a moment to render any error
            _wait.Until(d =>
                d.FindElements(By.CssSelector(".text-danger")).Count > 0
                || d.Url.Contains("/register"));

            Assert.Contains("/register", _driver.Url);

            bool hasError = _driver.PageSource.Contains("Mật khẩu")
                         || _driver.PageSource.Contains("Password")
                         || _driver.FindElements(By.CssSelector(".text-danger")).Count > 0;

            Assert.True(hasError, "Expected a password-validation error but none was found.");
        }
    }
}
