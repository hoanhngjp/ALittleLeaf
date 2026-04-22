using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System;
using System.IO;

namespace ALittleLeaf.FunctionalTests
{
    /// <summary>
    /// E2E admin product management tests against the React SPA at http://localhost:3000.
    ///
    /// URL map (legacy MVC → React SPA):
    ///   /Admin/Account/Login    → /login  (unified; role-based redirect to /admin)
    ///   /Admin/Products         → /admin/products
    ///   /Admin/Products/Create  → /admin/products/new
    ///   /Admin/Products/Edit/n  → /admin/products/:id
    ///
    /// Delete confirmation: window.confirm() (native browser alert), NOT a Bootstrap modal.
    /// </summary>
    public class ProductManagementE2ETests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string     _baseUrl = "http://localhost:3000";

        public ProductManagementE2ETests()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            _driver = new EdgeDriver(options);
        }

        public void Dispose() => _driver.Quit();

        /// <summary>
        /// Returns a wait that silently retries on NoSuchElement and StaleElement instead of
        /// aborting the polling loop.
        /// </summary>
        private WebDriverWait MakeWait(int seconds = 15)
        {
            var w = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
            w.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));
            return w;
        }

        /// <summary>
        /// Navigates to /login, fills the form, and presses Enter to submit.
        /// Using Keys.Return on the password field fires React's synthetic onSubmit
        /// reliably regardless of whether the click event would be intercepted.
        /// </summary>
        private void LoginAsAdmin()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/login");

            var wait = MakeWait(20);

            var emailInput = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("input[type='email']"));
                return el.Displayed && el.Enabled ? el : null;
            });
            emailInput!.SendKeys("hoanhnghiep2704@gmail.com");

            // Type password then press Enter — triggers React's onSubmit synchronously
            var pwInput = _driver.FindElement(By.CssSelector("input[type='password']"));
            pwInput.SendKeys("4L27hN04Aa@");
            pwInput.SendKeys(Keys.Return);

            // useLogin's onSuccess calls navigate('/admin') for role="admin"
            wait.Until(d => d.Url.Contains("/admin") && !d.Url.Contains("/login"));
        }

        // pm_001: Create product with valid data → appears in the list after search
        [Fact]
        public void CreateProduct_ValidData_ShouldAppearInList()
        {
            LoginAsAdmin();

            _driver.Navigate().GoToUrl($"{_baseUrl}/admin/products/new");

            string productName = "AutoTest Product " + Guid.NewGuid().ToString()[..8];

            var wait = MakeWait();

            // Wait for the form to mount before typing
            wait.Until(d => d.FindElement(By.Id("productName")).Displayed);

            _driver.FindElement(By.Id("productName")).SendKeys(productName);

            new OpenQA.Selenium.Support.UI.SelectElement(
                _driver.FindElement(By.Id("idCategory")))
                .SelectByIndex(0);

            _driver.FindElement(By.Id("productPrice")).SendKeys("100000");
            _driver.FindElement(By.Id("productDescription")).SendKeys("Mô tả sản phẩm tự động");
            _driver.FindElement(By.Id("quantityInStock")).SendKeys("50");

            var fileInput = wait.Until(d => d.FindElement(By.CssSelector("input[type='file']")));
            string imagePath = @"C:\Users\User\Downloads\download (26).png";
            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Test image not found: {imagePath}");
            fileInput.SendKeys(imagePath);

            // Submit — scroll into view then JS-click to avoid any overlay blocking
            var submitBtn = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("button.btn-primary[type='submit']"));
                return el.Displayed ? el : null;
            });
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            Thread.Sleep(400);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // After save the SPA navigates back to /admin/products
            wait.Until(d => d.Url.Contains("/admin/products") && !d.Url.Contains("/new"));

            // Type into the search input; React filters on keystroke — no search button needed
            var searchInput = wait.Until(d =>
                d.FindElement(By.CssSelector("input.form-control[placeholder*='Tìm']")));
            searchInput.Clear();
            searchInput.SendKeys(productName);

            wait.Until(d => d.PageSource.Contains(productName));
            Assert.True(_driver.PageSource.Contains(productName),
                $"Product '{productName}' not found in list after creation.");
        }

        // pm_002: Edit an existing product → updated name persists in the list
        [Fact]
        public void EditProduct_ChangePrice_ShouldUpdate()
        {
            LoginAsAdmin();
            _driver.Navigate().GoToUrl($"{_baseUrl}/admin/products");

            var wait = MakeWait();

            // Wait for at least one row to appear
            wait.Until(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 0);

            // Read the product name and click edit — both in a single fresh query to avoid
            // StaleElementReferenceException caused by React Query re-renders between calls
            string oldName = _driver
                .FindElement(By.CssSelector(".table tbody tr:first-child td:nth-of-type(2)"))
                .Text.Trim();

            _driver
                .FindElement(By.CssSelector(".table tbody tr:first-child button.btn-outline-primary"))
                .Click();

            // Now on /admin/products/:id (edit mode)
            wait.Until(d => d.Url.Contains("/admin/products/") && !d.Url.EndsWith("/new"));

            // Clear and update price
            var priceInput = wait.Until(d =>
            {
                var el = d.FindElement(By.Id("productPrice"));
                return el.Displayed && el.Enabled ? el : null;
            });
            priceInput!.Clear();
            priceInput.SendKeys("99999");

            // Clear and update name
            string newName = oldName + " Updated";
            var nameInput = _driver.FindElement(By.Id("productName"));
            nameInput.Clear();
            nameInput.SendKeys(newName);

            // Submit
            var submitBtn = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("button.btn-primary[type='submit']"));
                return el.Displayed ? el : null;
            });
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            Thread.Sleep(400);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // After save, SPA navigates back to the list
            wait.Until(d => d.Url.Contains("/admin/products") && !d.Url.Contains("/admin/products/"));

            // Re-locate the updated name — do NOT use any cached element from before navigation
            wait.Until(d => d.PageSource.Contains(newName));
            Assert.Contains(newName, _driver.PageSource);
        }

        // pm_003: Delete a product — AdminProductsPage uses window.confirm() (native browser alert)
        [Fact]
        public void DeleteProduct_ClickDelete_ShouldRemoveFromList()
        {
            LoginAsAdmin();
            _driver.Navigate().GoToUrl($"{_baseUrl}/admin/products");

            var wait = MakeWait();
            wait.Until(d => d.FindElements(By.CssSelector(".table tbody tr")).Count > 0);

            // Capture the product name before deleting (single fresh query)
            string deletedName = _driver
                .FindElement(By.CssSelector(".table tbody tr:first-child td:nth-of-type(2)"))
                .Text.Trim();

            // Click "Xóa" — AdminProductsPage calls window.confirm(), producing a native alert
            _driver
                .FindElement(By.CssSelector(".table tbody tr:first-child button.btn-outline-danger"))
                .Click();

            // Accept the native browser alert immediately — any delay risks UnhandledAlertException
            wait.Until(d =>
            {
                try { d.SwitchTo().Alert(); return true; }
                catch (NoAlertPresentException) { return false; }
            });
            _driver.SwitchTo().Alert().Accept();

            // React Query refetches the list; the deleted product should disappear
            wait.Until(d =>
            {
                try { return !d.PageSource.Contains(deletedName); }
                catch (UnhandledAlertException) { return false; }
            });

            Assert.DoesNotContain(deletedName, _driver.PageSource);
        }
    }
}
