using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALitlleLeaf.FunctionalTests
{
    public class OrderE2ETests
    {
        private IWebDriver CreateDriver()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");

            return new EdgeDriver(options);
        }

        private void Login(IWebDriver driver, string email, string password)
        {
            driver.Navigate().GoToUrl("http://localhost:8080/Account/Login");

            driver.FindElement(By.Id("email")).SendKeys(email);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.CssSelector("button.sign-in-btn")).Click();

            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => !d.Url.Contains("Login"));
        }

        private void SelectFirstAvailableProduct(IWebDriver driver)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            wait.Until(d => d.FindElements(By.CssSelector(".item-wrap")).Count > 0);

            var products = driver.FindElements(By.CssSelector(".item-wrap"));

            foreach (var product in products)
            {
                // Nếu KHÔNG có sold-out thì click
                var soldOut = product.FindElements(By.CssSelector(".sold-out"));
                if (soldOut.Count == 0)
                {
                    product.FindElement(By.CssSelector(".item-picture a")).Click();
                    return;
                }
            }

            throw new Exception("Không có sản phẩm còn hàng để test");
        }

        private void FillCheckoutAddress(IWebDriver driver, string address)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            wait.Until(d => d.FindElement(By.Id("BillingAddress")));
            driver.FindElement(By.Id("BillingAddress")).SendKeys(address);

            driver.FindElement(By.CssSelector("button.submit-order")).Click();
        }

        // ======================= ORDER TEST CASES =======================

        [Fact]
        [Trait("Category", "Order")]
        public void order_001_OrderSuccess_UntilPaymentPage()
        {
            var driver = CreateDriver();
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            try
            {
                // 1️⃣ Login
                Login(driver, "Thuong12@gmail.com", "Test@123");

                // 2️⃣ Vào Collections
                driver.Navigate().GoToUrl("http://localhost:8080/Collections");

                // Click sản phẩm đầu tiên
                wait.Until(d => d.FindElements(By.CssSelector(".item-picture a")).Count > 0);
                driver.FindElements(By.CssSelector(".item-picture a"))[1].Click(); // dùng index 0

                // 3️⃣ Trang Product → Add to Cart
                var addBtn = wait.Until(d =>
                {
                    var el = d.FindElement(By.CssSelector(".addToCartProduct")); // đúng class trong view
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                addBtn.Click();

                // 4️⃣ Vào Cart
                driver.Navigate().GoToUrl("http://localhost:8080/Cart");

                // Click Checkout
                var checkoutBtn = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id("checkout")); // hoặc By.CssSelector(".btn-checkout")
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                checkoutBtn.Click();

                // 5️⃣ Trang Checkout → nhập địa chỉ
                var fullNameInput = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id("billing_address_full_name"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                fullNameInput.SendKeys("Nguyễn Văn A");

                var phoneInput = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id("billing_address_phone"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                phoneInput.SendKeys("0912345678");

                var addressInput = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id("billing_address_address"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                addressInput.SendKeys("Dương Bá Trạc, Quận 8");

                // Click nút Tiếp tục
                var continueBtn = wait.Until(d =>
                {
                    var el = d.FindElement(By.CssSelector(".step-footer-continue-btn"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                continueBtn.Click();

                // 6️⃣ Assert chuyển sang Payment
                wait.Until(d => d.Url.Contains("Payment"));
                Assert.Contains("Payment", driver.Url);
            }
            finally
            {
                driver.Quit();
            }
        }
        [Fact]
        [Trait("Category", "Order")]
        public void order_002_OrderSuccess_PaymentCompleted()
        {
            var driver = CreateDriver();
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60)); // Tăng timeout để an toàn

            try
            {
                // ... (Các bước 1 đến 8: Login -> Mua hàng -> Chọn VNPay -> Redirect giữ nguyên) ...

                // 1️⃣ Login
                Login(driver, "Thuong12@gmail.com", "Test@123");
                // 2️⃣ Vào Collections
                driver.Navigate().GoToUrl("http://localhost:8080/Collections");
                // 3️⃣ Chọn sản phẩm
                SelectFirstAvailableProduct(driver);
                // 4️⃣ Mua ngay
                var buyNowBtn = wait.Until(d => d.FindElement(By.CssSelector(".addToCartProduct")));
                buyNowBtn.Click();
                // 5️⃣ Checkout
                driver.Navigate().GoToUrl("http://localhost:8080/Cart");
                wait.Until(d => d.FindElement(By.Id("checkout"))).Click();
                // 6️⃣ Nhập địa chỉ
                wait.Until(d => d.FindElement(By.Id("billing_address_full_name"))).SendKeys("NGUYEN VAN A");
                wait.Until(d => d.FindElement(By.Id("billing_address_phone"))).SendKeys("0912345678");
                wait.Until(d => d.FindElement(By.Id("billing_address_address"))).SendKeys("HCM");
                wait.Until(d => d.FindElement(By.CssSelector(".step-footer-continue-btn"))).Click();
                // 7️⃣ Chọn Payment Method
                var vnpayRadio = wait.Until(d => d.FindElement(By.Id("vnpay_method")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", vnpayRadio);
                wait.Until(d => d.FindElement(By.Id("btn-complete-order"))).Click();

                // 8️⃣ Chờ Redirect sang VNPay
                wait.Until(d => d.Url.Contains("vnpay"));

                // =======================================================
                // XỬ LÝ GIAO DIỆN VNPAY (CẬP NHẬT SELECTOR)
                // =======================================================

                // 9️⃣ Switch iframe (nếu có - quan trọng)
                // VNPAY Sandbox có thể dùng hoặc không dùng iframe tùy phiên bản/cấu hình.
                // Code này sẽ tự động detect.
                if (driver.FindElements(By.TagName("iframe")).Count > 0)
                {
                    driver.SwitchTo().Frame(0);
                }

                // 🔟 Mở tab "Thẻ nội địa"
                // Dùng CSS Selector chính xác theo HTML bạn cung cấp
                var domesticTab = wait.Until(d => d.FindElement(By.CssSelector("[data-bs-target='#accordionList2']")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", domesticTab);

                // 1️⃣1️⃣ Chọn Ngân hàng NCB (Cơ chế Retry)
                // Đợi danh sách ngân hàng hiện ra (có thể bị delay do animation collapse)
                // Dùng Try-Catch để retry click nếu animation chưa xong
                bool isBankSelected = false;
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var ncbBtn = wait.Until(d => d.FindElement(By.Id("NCB")));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ncbBtn);

                        // Kiểm tra xem ô nhập thẻ đã hiện chưa để xác nhận click thành công
                        // ID mới: card_number_mask
                        var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                        shortWait.Until(d => d.FindElement(By.Id("card_number_mask")).Displayed);

                        isBankSelected = true;
                        break;
                    }
                    catch (WebDriverTimeoutException)
                    {
                        Thread.Sleep(1000); // Đợi 1s rồi thử lại
                    }
                }

                if (!isBankSelected) throw new Exception("Không thể chọn ngân hàng NCB.");

                // 1️⃣2️⃣ Nhập thông tin thẻ (Dùng ID mới)
                var cardInput = driver.FindElement(By.Id("card_number_mask"));
                cardInput.Clear();
                cardInput.SendKeys("9704198526191432198"); // Thẻ Test NCB

                var nameInput = driver.FindElement(By.Id("cardHolder"));
                nameInput.Clear();
                nameInput.SendKeys("NGUYEN VAN A");

                var dateInput = driver.FindElement(By.Id("cardDate"));
                dateInput.Clear();
                dateInput.SendKeys("07/15");

                // 1️⃣3️⃣ Bấm Tiếp tục (Dùng ID mới: btnContinue)
                var btnContinue = driver.FindElement(By.Id("btnContinue"));
                // Đôi khi nút bị che bởi bàn phím ảo hoặc footer, dùng JS click cho chắc
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnContinue);

                // =======================================================
                // XỬ LÝ OTP (Màn hình 2)
                // =======================================================

                // 1️⃣4️⃣ Chờ ô OTP (Dùng ID mới: otpvalue)
                var otpInput = wait.Until(d => d.FindElement(By.Id("otpvalue")));
                otpInput.SendKeys("123456");

                // 1️⃣5️⃣ Bấm Thanh toán (Dùng ID mới: btnConfirm)
                var btnConfirm = wait.Until(d => d.FindElement(By.Id("btnConfirm")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnConfirm);

                // =======================================================
                // VERIFY KẾT QUẢ (Về lại Web của bạn)
                // =======================================================

                driver.SwitchTo().DefaultContent(); // Thoát iframe

                // Chờ quay về local (URL chứa Callback hoặc trang Success)
                wait.Until(d => d.Url.Contains("PaymentCallbackVnpay") || d.Url.Contains("PaymentSuccess"));

                Assert.True(driver.PageSource.Contains("thành công") || driver.PageSource.Contains("Success") || driver.Url.Contains("Success"),
                    "Không thấy thông báo thành công");
            }
            catch (Exception)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile($"Error_Payment_{DateTime.Now.Ticks}.png");
                throw;
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
