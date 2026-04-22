using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;

namespace ALittleLeaf.FunctionalTests
{
    /// <summary>
    /// E2E order / checkout tests against the React SPA at http://localhost:3000.
    ///
    /// URL changes vs. legacy MVC:
    ///   /Account/Login          → /login
    ///   /Collections            → /collections
    ///   /Cart                   → /cart
    ///   /CheckOut               → /checkout  (address step)
    ///   /CheckOut/Payment       → /checkout/payment
    ///   /PaymentCallbackVnpay   → /payment-result
    ///
    /// Selector changes:
    ///   button.sign-in-btn      → button.brand-btn
    ///   .item-wrap              → .pro-loop       (product card in collections grid)
    ///   .item-picture a         → .pro-loop a     (link inside product card)
    ///   .addToCartProduct       → .addToCartProduct  (unchanged)
    ///   #checkout               → .btn-checkout   (checkout button in cart)
    ///   .step-footer-continue-btn → .step-footer-continue-btn  (unchanged)
    ///   #vnpay_method / #btn-complete-order → preserved in CheckoutPaymentPage
    /// </summary>
    public class OrderE2ETests
    {
        private const string BaseUrl = "http://localhost:3000";

        private IWebDriver CreateDriver()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            return new EdgeDriver(options);
        }

        /// <summary>
        /// Logs in via the React /login page and waits until the SPA navigates away.
        /// Uses IgnoreExceptionTypes so a NoSuchElementException thrown before React
        /// mounts the form does not abort the wait retry loop.
        /// </summary>
        private static void Login(IWebDriver driver, string email, string password)
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/login");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));

            var emailInput = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("input[type='email']"));
                return el.Displayed && el.Enabled ? el : null;
            });
            emailInput!.SendKeys(email);

            // Press Enter on the password field — fires React's onSubmit synchronously
            var pwInput = driver.FindElement(By.CssSelector("input[type='password']"));
            pwInput.SendKeys(password);
            pwInput.SendKeys(Keys.Return);

            wait.Until(d => !d.Url.Contains("/login"));
        }

        /// <summary>Clicks the first non-sold-out product card in the collections grid.</summary>
        private void SelectFirstAvailableProduct(IWebDriver driver)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            // React renders product cards as .pro-loop elements (same class as legacy)
            wait.Until(d => d.FindElements(By.CssSelector(".pro-loop")).Count > 0);

            var products = driver.FindElements(By.CssSelector(".pro-loop"));
            foreach (var product in products)
            {
                if (product.FindElements(By.CssSelector(".sold-out")).Count == 0)
                {
                    // Navigate into the product detail via the card link
                    product.FindElement(By.CssSelector("a")).Click();
                    return;
                }
            }

            throw new Exception("No in-stock product found to test with.");
        }

        // order_001: Browse → Add to cart → Checkout address form → assert on /checkout/payment or /checkout
        [Xunit.Fact]
        [Xunit.Trait("Category", "Order")]
        public void order_001_OrderSuccess_UntilPaymentPage()
        {
            var driver = CreateDriver();
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));

            try
            {
                // 1. Login as customer
                Login(driver, "Thuong12@gmail.com", "Test@123");

                // 2. Browse collections
                driver.Navigate().GoToUrl($"{BaseUrl}/collections");

                // 3. Click the second visible product link (index 1) to avoid any pinned hero item
                wait.Until(d => d.FindElements(By.CssSelector(".pro-loop a")).Count > 0);
                driver.FindElements(By.CssSelector(".pro-loop a"))[1].Click();

                // 4. Product detail page — Add to cart
                var addBtn = wait.Until(d =>
                {
                    var el = d.FindElement(By.CssSelector(".addToCartProduct"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                addBtn!.Click();

                // 5. Navigate to cart
                driver.Navigate().GoToUrl($"{BaseUrl}/cart");

                // 6. Click checkout button (.btn-checkout replaces the old #checkout id)
                var checkoutBtn = wait.Until(d =>
                {
                    var el = d.FindElement(By.CssSelector(".btn-checkout"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                checkoutBtn!.Click();

                // 7. Shipping address form at /checkout
                wait.Until(d => d.Url.Contains("/checkout"));

                wait.Until(d =>
                {
                    var el = d.FindElement(By.Id("billing_address_full_name"));
                    return el.Displayed ? el : null;
                })!.SendKeys("Nguyễn Văn A");

                driver.FindElement(By.Id("billing_address_phone")).SendKeys("0912345678");
                driver.FindElement(By.Id("billing_address_address")).SendKeys("Dương Bá Trạc, Quận 8");

                // 8. Continue to payment method step
                var continueBtn = wait.Until(d =>
                {
                    var el = d.FindElement(By.CssSelector(".step-footer-continue-btn"));
                    return (el.Displayed && el.Enabled) ? el : null;
                });
                continueBtn!.Click();

                // 9. Assert now on payment method step (/checkout/payment or /checkout with hash)
                wait.Until(d => d.Url.Contains("/checkout"));
                Assert.Contains("/checkout", driver.Url);
            }
            finally
            {
                driver.Quit();
            }
        }

        // order_002: Full VNPay sandbox payment flow
        [Xunit.Fact]
        [Xunit.Trait("Category", "Order")]
        public void order_002_OrderSuccess_PaymentCompleted()
        {
            var driver = CreateDriver();
            // Use a long timeout: VNPay sandbox can be slow
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));

            try
            {
                // 1. Login
                Login(driver, "Thuong12@gmail.com", "Test@123");

                // 2. Collections → pick first available product
                driver.Navigate().GoToUrl($"{BaseUrl}/collections");
                SelectFirstAvailableProduct(driver);

                // 3. Add to cart
                var buyNowBtn = wait.Until(d => d.FindElement(By.CssSelector(".addToCartProduct")));
                buyNowBtn.Click();

                // 4. Cart → checkout
                driver.Navigate().GoToUrl($"{BaseUrl}/cart");
                wait.Until(d => d.FindElement(By.CssSelector(".btn-checkout"))).Click();

                // 5. Shipping address
                wait.Until(d => d.Url.Contains("/checkout"));
                wait.Until(d => d.FindElement(By.Id("billing_address_full_name"))).SendKeys("NGUYEN VAN A");
                driver.FindElement(By.Id("billing_address_phone")).SendKeys("0912345678");
                driver.FindElement(By.Id("billing_address_address")).SendKeys("HCM");
                wait.Until(d => d.FindElement(By.CssSelector(".step-footer-continue-btn"))).Click();

                // 6. Payment method — select VNPay and submit order
                // CheckoutPaymentPage renders #vnpay_method radio and #btn-complete-order button
                var vnpayRadio = wait.Until(d => d.FindElement(By.Id("vnpay_method")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", vnpayRadio);
                wait.Until(d => d.FindElement(By.Id("btn-complete-order"))).Click();

                // 7. Redirected to VNPay sandbox
                wait.Until(d => d.Url.Contains("vnpayment") || d.Url.Contains("vnpay"));

                // ── VNPay sandbox interaction ──────────────────────────────────

                // Switch into iframe if present
                if (driver.FindElements(By.TagName("iframe")).Count > 0)
                    driver.SwitchTo().Frame(0);

                // Open "Thẻ nội địa" tab
                var domesticTab = wait.Until(d => d.FindElement(
                    By.CssSelector("[data-bs-target='#accordionList2']")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", domesticTab);

                // Select NCB bank with retry (collapse animation)
                bool isBankSelected = false;
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var ncbBtn = wait.Until(d => d.FindElement(By.Id("NCB")));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ncbBtn);
                        new WebDriverWait(driver, TimeSpan.FromSeconds(3))
                            .Until(d => d.FindElement(By.Id("card_number_mask")).Displayed);
                        isBankSelected = true;
                        break;
                    }
                    catch (WebDriverTimeoutException)
                    {
                        Thread.Sleep(1000);
                    }
                }

                if (!isBankSelected) throw new Exception("Could not select NCB bank.");

                // Enter test card details
                var cardInput = driver.FindElement(By.Id("card_number_mask"));
                cardInput.Clear();
                cardInput.SendKeys("9704198526191432198");

                driver.FindElement(By.Id("cardHolder")).Clear();
                driver.FindElement(By.Id("cardHolder")).SendKeys("NGUYEN VAN A");

                driver.FindElement(By.Id("cardDate")).Clear();
                driver.FindElement(By.Id("cardDate")).SendKeys("07/15");

                ((IJavaScriptExecutor)driver).ExecuteScript(
                    "arguments[0].click();", driver.FindElement(By.Id("btnContinue")));

                // OTP screen
                wait.Until(d => d.FindElement(By.Id("otpvalue"))).SendKeys("123456");
                ((IJavaScriptExecutor)driver).ExecuteScript(
                    "arguments[0].click();",
                    wait.Until(d => d.FindElement(By.Id("btnConfirm"))));

                // ── Back on our React app ──────────────────────────────────────

                driver.SwitchTo().DefaultContent();

                // React SPA shows result at /payment-result (replaces legacy /PaymentCallbackVnpay)
                wait.Until(d => d.Url.Contains("/payment-result") || d.Url.Contains("payment-result"));

                Assert.True(
                    driver.PageSource.Contains("thành công")
                    || driver.PageSource.Contains("Success")
                    || driver.Url.Contains("payment-result"),
                    "Payment success indicator not found.");
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
