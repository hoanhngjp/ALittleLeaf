using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;


namespace ALittleLeaf.Controllers
{
    public class CartController : Controller
    {
        private readonly AlittleLeafDecorContext db;
        public CartController(AlittleLeafDecorContext context) => db = context;
        
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            var totalPrice = cart.Sum(c => c.Quantity * c.ProductPrice);

            ViewBag.TotalPrice = totalPrice;

            ViewData["Cart"] = cart;
            // Truyền giỏ hàng đến View
            return View();
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }
            // Lấy sản phẩm từ database
            var product = db.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new CartItemViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    Quantity = quantity,
                    ProductImg = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName
                })
                .FirstOrDefault();

            if (product == null)
            {
                // Xử lý nếu không tìm thấy sản phẩm
                return NotFound();
            }

            // Lấy giỏ hàng từ Session (nếu giỏ hàng chưa tồn tại, tạo mới)
            List<CartItemViewModel> cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                // Cập nhật số lượng nếu đã có trong giỏ
                existingItem.Quantity += quantity;
            }
            else
            {
                // Thêm mới sản phẩm vào giỏ hàng
                cart.Add(product);
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            // Kiểm tra tham số truyền vào
            Console.WriteLine($"ProductId: {productId}, Quantity: {quantity}");

            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart");

            if (cart == null || cart.Count == 0)
            {
                return Json(new { success = false, message = "Giỏ hàng rỗng" });
            }

            // Tìm sản phẩm trong giỏ hàng
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }

            // Cập nhật số lượng sản phẩm
            item.Quantity = quantity;

            // Tính toán lại tổng tiền của sản phẩm
            var lineItemTotal = item.Quantity * item.ProductPrice;

            // Tính lại tổng tiền giỏ hàng
            var totalPrice = cart.Sum(c => c.Quantity * c.ProductPrice);

            // Lưu giỏ hàng lại vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Trả về dữ liệu tổng tiền mới
            return Json(new
            {
                success = true,
                lineItemTotal = lineItemTotal,
                total_price = totalPrice
            });
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            // Giả sử bạn đang lưu giỏ hàng trong session hoặc cơ sở dữ liệu
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart");

            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);  // Xóa sản phẩm khỏi giỏ hàng
                HttpContext.Session.SetObjectAsJson("Cart", cart);  // Cập nhật lại giỏ hàng trong session
            }

            // Tính lại tổng tiền giỏ hàng
            var totalPrice = cart.Sum(item => item.Quantity * item.ProductPrice);

            return Json(new { success = true, total_price = totalPrice });
        }
        [HttpPost]
        public IActionResult GetCartNote(string? BillNote) 
        {
            if(BillNote == null)
            {
                return RedirectToAction("Index", "OrderInfo");
            }
            else
            {
                HttpContext.Session.SetString("BillNote", BillNote);
                return RedirectToAction("Index", "OrderInfo");
            }    
        }
    }
}
