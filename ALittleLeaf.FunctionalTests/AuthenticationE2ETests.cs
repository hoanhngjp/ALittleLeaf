using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace ALittleLeaf.FunctionalTests
{
    /// <summary>
    /// E2E authentication tests against the React SPA at http://localhost:3000.
    ///
    /// Key selectors (confirmed from LoginPage.jsx):
    ///   Email input    — input[type='email']   (also has id="email")
    ///   Password input — input[type='password'] (also has id="password")
    ///   Submit button  — button[type='submit']  (also has class "btn brand-btn")
    ///
    /// All WebDriverWait instances call IgnoreExceptionTypes so that a
    /// NoSuchElementException thrown before React mounts the form does NOT
    /// abort the retry loop — the wait simply polls again on the next interval.
    /// </summary>
    public class AuthenticationE2ETests
    {
        private const string BaseUrl = "http://localhost:3000";

        private static IWebDriver CreateDriver()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            return new EdgeDriver(options);
        }

        /// <summary>
        /// Creates a WebDriverWait that silently retries on NoSuchElementException
        /// and StaleElementReferenceException instead of aborting immediately.
        /// </summary>
        private static WebDriverWait MakeWait(IWebDriver driver, int seconds = 20)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));
            return wait;
        }

        /// <summary>
        /// Fills the login form and submits by pressing Enter on the password field.
        /// Keys.Return fires React's synthetic onSubmit reliably; a bare .Click() on the
        /// submit button can fire before React's controlled-input state has committed.
        /// </summary>
        private static void FillAndSubmitLogin(IWebDriver driver, string email, string password)
        {
            var wait = MakeWait(driver);

            var emailInput = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("input[type='email']"));
                return el.Displayed && el.Enabled ? el : null;
            });
            emailInput!.Clear();
            emailInput.SendKeys(email);

            // Type password then press Enter — triggers onSubmit without a separate click
            var pwInput = driver.FindElement(By.CssSelector("input[type='password']"));
            pwInput.SendKeys(password);
            pwInput.SendKeys(Keys.Return);
        }

        // login_001: Customer login with valid credentials → redirected away from /login
        [Fact]
        public void Login_With_Valid_Account_Should_Success()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/login");
                FillAndSubmitLogin(driver, "Thuong12@gmail.com", "Test@123");

                var wait = MakeWait(driver);
                wait.Until(d => !d.Url.Contains("/login"));

                Assert.DoesNotContain("/login", driver.Url);
            }
            finally { driver.Quit(); }
        }

        // login_002: Admin login → useLogin redirects to /admin based on role
        [Fact]
        public void login_002_AdminLoginSuccess_RedirectToAdminDashboard()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/login");
                FillAndSubmitLogin(driver, "hoanhnghiep2704@gmail.com", "4L27hN04Aa@");

                var wait = MakeWait(driver);
                wait.Until(d => d.Url.Contains("/admin"));

                Assert.Contains("/admin", driver.Url);
            }
            finally { driver.Quit(); }
        }

        // login_003: Wrong password → API returns 401, React shows alert-danger, URL stays /login
        [Fact]
        public void login_003_WrongPassword_StayOnLoginPage()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/login");
                FillAndSubmitLogin(driver, "Thuong12@gmail.com", "WrongPass@999");

                var wait = MakeWait(driver);
                // LoginPage.jsx renders the server error inside <div className="alert alert-danger ...">
                wait.Until(d => d.FindElements(By.CssSelector(".alert-danger")).Count > 0);

                Assert.Contains("/login", driver.Url);
            }
            finally { driver.Quit(); }
        }

        // login_004: Non-existent email → same 401 path
        [Fact]
        public void login_004_EmailNotExist_StayOnLoginPage()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/login");
                FillAndSubmitLogin(driver, "nonexistent_xyz@gmail.com", "AnyPass@123");

                var wait = MakeWait(driver);
                wait.Until(d => d.FindElements(By.CssSelector(".alert-danger")).Count > 0);

                Assert.Contains("/login", driver.Url);
            }
            finally { driver.Quit(); }
        }

        // login_005: Locked/inactive account → same 401 path
        [Fact]
        public void login_005_AccountLocked_StayOnLoginPage()
        {
            var driver = CreateDriver();
            try
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/login");
                FillAndSubmitLogin(driver, "Thuong124@gmail.com", "Test@123");

                var wait = MakeWait(driver);
                wait.Until(d => d.FindElements(By.CssSelector(".alert-danger")).Count > 0);

                Assert.Contains("/login", driver.Url);
            }
            finally { driver.Quit(); }
        }
    }
}
