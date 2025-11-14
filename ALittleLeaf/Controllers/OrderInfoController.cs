using ALittleLeaf.Filters;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ALittleLeaf.Controllers
{
    [CheckLogin]
    public class OrderInfoController : SiteBaseController
    {
        private readonly AlittleLeafDecorContext _context;

        public OrderInfoController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy giỏ hàng
            var cart = ViewData["Cart"] as List<ALittleLeaf.ViewModels.CartItemViewModel>;
            
            ViewData["Cart"] = cart;

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Collections");
            }

            var userIdString = HttpContext.Session.GetString("UserId");

            long userId = long.Parse(userIdString);

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
                adrs_id = address.AdrsId,
                adrs_fullname = address.AdrsFullname,
                adrs_phone = address.AdrsPhone,
                adrs_address = address.AdrsAddress
            });
        }
        [HttpPost]
        public IActionResult SaveAddressInfo(string bill_addressId, string billing_address_full_name, string billing_address_phone, string billing_address_address)
        {
            // Lưu thông tin vào session
            if (bill_addressId == null)
            {
                HttpContext.Session.SetString("BillingAdrsId", "new");
            }
            else
            {
                HttpContext.Session.SetString("BillingAdrsId", bill_addressId);
            }
            HttpContext.Session.SetString("BillingFullName", billing_address_full_name);
            HttpContext.Session.SetString("BillingPhone", billing_address_phone);
            HttpContext.Session.SetString("BillingAddress", billing_address_address);

            //Check
            Console.WriteLine(bill_addressId);
            Console.WriteLine(billing_address_full_name);
            Console.WriteLine(billing_address_phone);
            Console.WriteLine(billing_address_address);

            // Trả về phản hồi thành công
            return RedirectToAction("Index", "CheckOut");
        }

    }
}
