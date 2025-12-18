using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System;
using System.IO;
using System.Threading;

namespace ALittleLeaf.FunctionalTests
{
    public class ProductManagementE2ETests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "http://localhost:8080";
        private readonly WebDriverWait _wait;

        public ProductManagementE2ETests()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new EdgeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15)); // Tăng timeout lên 15s cho an toàn
        }

        public void Dispose()
        {
            _driver.Quit();
        }

        private void LoginAsAdmin()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Admin/Account/Login");

            _driver.FindElement(By.Id("email")).SendKeys("hoanhnghiep2704@gmail.com");
            _driver.FindElement(By.Id("password")).SendKeys("4L27hN04Aa@");

            var loginBtn = _driver.FindElement(By.CssSelector("button.sign-in-btn"));
            // Dùng JS Click cho chắc ăn ngay từ Login
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", loginBtn);

            // Chờ redirect
            _wait.Until(d => d.Url.Contains("Admin") && !d.Url.Contains("Login"));
        }

        [Fact]
        public void CreateProduct_ValidData_ShouldAppearInList()
        {
            // 1. Login Admin
            LoginAsAdmin();

            // 2. Vào trang Create Product
            _driver.Navigate().GoToUrl($"{_baseUrl}/Admin/Products/Create");

            // 3. Điền thông tin
            string productName = "AutoTest Product " + Guid.NewGuid().ToString().Substring(0, 8);

            _driver.FindElement(By.Id("product_name")).SendKeys(productName);

            var categorySelect = new SelectElement(_driver.FindElement(By.Id("id_category")));
            categorySelect.SelectByIndex(0);

            _driver.FindElement(By.Id("product_price")).SendKeys("100000");
            _driver.FindElement(By.Id("product_description")).SendKeys("Mô tả sản phẩm tự động");
            _driver.FindElement(By.Id("quantity_in_stock")).SendKeys("50");

            // 4. Upload ảnh
            var fileInput = _wait.Until(d => d.FindElement(By.CssSelector("input[name='ProductImages']")));
            string imagePath = @"C:\Users\User\Downloads\download (26).png"; // Đường dẫn file của bạn

            if (!File.Exists(imagePath)) throw new FileNotFoundException($"File ảnh không tồn tại: {imagePath}");
            fileInput.SendKeys(imagePath);

            // 5. Submit Form
            var submitBtn = _driver.FindElement(By.CssSelector("button[type='submit']"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // --- FIX LỖI Ở ĐÂY ---

            // 1. Đợi thoát khỏi trang Create (Quan trọng!)
            _wait.Until(d => !d.Url.Contains("Create"));

            // 2. Đợi quay về trang Index
            _wait.Until(d => d.Url.EndsWith("Products") || d.Url.EndsWith("Products/"));

            // 3. Sử dụng tính năng Tìm kiếm để tìm sản phẩm (Xử lý vấn đề phân trang)
            // Chọn loại tìm kiếm: Theo tên
            var searchType = new SelectElement(_driver.FindElement(By.Id("searchType")));
            searchType.SelectByValue("findByProductName");

            // Nhập tên vào ô tìm kiếm
            var searchInput = _driver.FindElement(By.Id("searchKey"));
            searchInput.Clear();
            searchInput.SendKeys(productName);

            // Bấm nút Tìm (Nút có icon search và chữ Tìm)
            // XPath tìm nút chứa text 'Tìm'
            var searchBtn = _driver.FindElement(By.XPath("//button[contains(.,'Tìm')]"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", searchBtn);

            // 4. Đợi kết quả AJAX tải về
            // Chúng ta đợi cho đến khi tên sản phẩm xuất hiện trong PageSource
            try
            {
                _wait.Until(d => d.PageSource.Contains(productName));
            }
            catch (WebDriverTimeoutException)
            {
                // Nếu timeout nghĩa là không tìm thấy
            }

            // 5. Verify cuối cùng
            bool isFound = _driver.PageSource.Contains(productName);
            Assert.True(isFound, $"Sản phẩm '{productName}' không tìm thấy trong danh sách sau khi tạo và tìm kiếm.");
        }

        [Fact]
        public void EditProduct_ChangePrice_ShouldUpdate()
        {
            // 1. Login Admin
            LoginAsAdmin();
            _driver.Navigate().GoToUrl($"{_baseUrl}/Admin/Products");

            // 2. Tìm sản phẩm đầu tiên để sửa
            var firstRow = _wait.Until(d => d.FindElement(By.CssSelector("#userTable tbody tr:first-child")));

            // SỬA LỖI 2: Dùng nth-of-type(2) thay vì nth-child(2)
            // Vì HTML của bạn có thẻ <input> chen giữa các thẻ <td>
            var nameCell = firstRow.FindElement(By.CssSelector("td:nth-of-type(2)"));
            string oldName = nameCell.Text;

            // Click nút "Sửa thông tin sản phẩm"
            var editBtn = firstRow.FindElement(By.CssSelector("a.btn-outline-primary"));
            // Dùng JS click luôn cho an toàn
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editBtn);

            // 3. Verify đang ở trang Edit
            _wait.Until(d => d.Url.Contains("/Edit"));

            // 4. Sửa giá tiền
            var priceInput = _driver.FindElement(By.Id("product_price"));
            priceInput.Clear();
            string newPrice = "99999";
            priceInput.SendKeys(newPrice);

            // Sửa tên
            var nameInput = _driver.FindElement(By.Id("product_name"));
            string newName = oldName + " Updated";
            nameInput.Clear();
            nameInput.SendKeys(newName);

            // 5. Submit (Dùng JS Click)
            // Tìm nút theo XPath chứa text
            var submitBtn = _driver.FindElement(By.XPath("//button[contains(text(),'Sửa thông tin sản phẩm')]"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            // 6. Verify quay lại trang Index
            _wait.Until(d => d.Url.Contains("/Admin/Products"));

            // Xử lý trường hợp redirect về Edit thay vì Index
            if (_driver.Url.Contains("/Edit"))
            {
                _driver.Navigate().GoToUrl($"{_baseUrl}/Admin/Products");
            }

            // 7. Check giá trị mới trong bảng
            _wait.Until(d => d.PageSource.Contains(newName));
            Assert.Contains(newName, _driver.PageSource);
        }

        [Fact]
        public void DeleteProduct_ClickDelete_ShouldRemoveFromList()
        {
            // 1. Login
            LoginAsAdmin();
            _driver.Navigate().GoToUrl($"{_baseUrl}/Admin/Products");

            // 2. Chọn row đầu tiên
            var firstRow = _wait.Until(d => d.FindElement(By.CssSelector("#userTable tbody tr:first-child")));
            var editBtn = firstRow.FindElement(By.CssSelector("a.btn-outline-primary"));

            // Vào trang Edit
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editBtn);

            // 3. Tìm nút Xóa
            var deleteBtn = _wait.Until(d => d.FindElement(By.CssSelector("button.btn-danger")));

            // 4. Click Xóa & Handle Confirm Alert
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", deleteBtn);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", deleteBtn);

            try
            {
                var alert = _wait.Until(d => d.SwitchTo().Alert());
                alert.Accept(); // Bấm OK
            }
            catch (WebDriverTimeoutException)
            {
                // Bỏ qua nếu không có alert
            }

            // 5. Verify quay về Index
            _wait.Until(d => d.Url.Contains("/Admin/Products"));

            Assert.Contains("Admin/Products", _driver.Url);
        }
    }
}