using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ALittleLeaf.Controllers
{
    public class OrderInfoController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public OrderInfoController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            ViewData["Cart"] = cart;

            // Lấy danh sách địa chỉ của người dùng hiện tại (giả sử IdUser được lưu trong Session)
            var userIdString = HttpContext.Session.GetString("UserId");

            long userId = long.Parse(userIdString); // Chuyển đổi chuỗi thành kiểu long

            var addresses = _context.AddressLists
                .Where(a => a.IdUser == userId)
                .ToList();

            ViewData["Addresses"] = addresses;

            return View();
        }
        [HttpPost]
        public IActionResult GetAddressInfo(int addressId)
        {
            Console.WriteLine($"Received AddressId: {addressId}");

            if (addressId <= 0)
            {
                return Json(new { error = "Invalid address ID" });
            }

            var address = _context.AddressLists.FirstOrDefault(a => a.AdrsId == addressId);

            if (address == null)
            {
                return Json(new { error = "Address not found" });
            }

            return Json(new
            {
                adrs_fullname = address.AdrsFullname,
                adrs_phone = address.AdrsPhone,
                adrs_address = address.AdrsAddress
            });
        }
        [HttpPost]
        public IActionResult SaveAddressInfo(string billing_address_full_name, string billing_address_phone, string billing_address_address)
        {
            // Lưu thông tin vào session
            HttpContext.Session.SetString("BillingFullName", billing_address_full_name);
            HttpContext.Session.SetString("BillingPhone", billing_address_phone);
            HttpContext.Session.SetString("BillingAddress", billing_address_address);

            // Trả về phản hồi thành công
            return View();
        }

    }
}
