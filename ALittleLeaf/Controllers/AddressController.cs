using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class AddressController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AddressController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Lấy danh sách địa chỉ từ cơ sở dữ liệu
            var addresses = _context.AddressLists
                .Where(a => a.IdUser == long.Parse(userId))
                .ToList();

            ViewData["Addresses"] = addresses;

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
            ViewData["Cart"] = cart;

            return View();
        }
        [HttpPost]
        public IActionResult EditAddress(int AdrsId, string AdrsFullname, string AdrsAddress, string AdrsPhone, int AdrsIsDefault)
        {
            try
            {
                // Tìm địa chỉ cần cập nhật theo AdrsId
                var address = _context.AddressLists.FirstOrDefault(a => a.AdrsId == AdrsId);

                if (address == null)
                {
                    // Nếu không tìm thấy địa chỉ
                    return NotFound("Address not found.");
                }
                Console.WriteLine(AdrsIsDefault);
                // Cập nhật thông tin địa chỉ
                address.AdrsFullname = AdrsFullname;
                address.AdrsAddress = AdrsAddress;
                address.AdrsPhone = AdrsPhone;
                if (AdrsIsDefault == 1)
                {
                    address.AdrsIsDefault = true;
                }
                else
                {
                    address.AdrsIsDefault = false;
                }
                Console.WriteLine(address.AdrsFullname);
                Console.WriteLine(address.AdrsAddress);
                Console.WriteLine(address.AdrsPhone);
                Console.WriteLine(address.AdrsIsDefault);
                Console.WriteLine(address.IdUser);
                // Nếu địa chỉ được đặt làm mặc định, bỏ cờ mặc định ở các địa chỉ khác
                if (address.AdrsIsDefault)
                {
                    var userId = address.IdUser; // Giả sử IdUser là khóa ngoại trong AddressList
                    Console.WriteLine(userId);
                    var otherAddresses = _context.AddressLists.Where(a => a.IdUser == userId && a.AdrsId != AdrsId);

                    foreach (var other in otherAddresses)
                    {
                        other.AdrsIsDefault = false;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Trả về thông báo thành công (hoặc điều hướng)
                return Json(new { success = true, message = "Cập nhật thông tin địa chỉ thành công" });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        [HttpPost]
        public IActionResult DeleteAddress(int AdrsId)
        {
            try
            {
                // Tìm địa chỉ trong cơ sở dữ liệu
                var address = _context.AddressLists.FirstOrDefault(a => a.AdrsId == AdrsId);

                if (address == null)
                {
                    // Nếu không tìm thấy địa chỉ
                    return Json(new { success = false, message = "Địa chỉ không tồn tại." });
                }

                // Xóa địa chỉ
                _context.AddressLists.Remove(address);
                _context.SaveChanges();

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Địa chỉ đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return Json(new { success = false, message = $"Đã có lỗi xảy ra: {ex.Message}" });
            }
        }
        [HttpPost]
        public IActionResult AddAddress(string AdrsFullname, string AdrsAddress, string AdrsPhone, int AdrsIsDefault)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                // Tạo đối tượng Address mới
                var newAddress = new AddressList
                {
                    AdrsFullname = AdrsFullname,
                    AdrsAddress = AdrsAddress,
                    AdrsPhone = AdrsPhone,
                    AdrsIsDefault = AdrsIsDefault == 1, // Chuyển đổi thành kiểu bool
                    IdUser = long.Parse(userId) // Giả sử bạn có cách lấy ID người dùng hiện tại
                };

                // Nếu địa chỉ mới được đặt làm mặc định, cập nhật các địa chỉ khác
                if (newAddress.AdrsIsDefault)
                {
                    var otherAddresses = _context.AddressLists.Where(a => a.IdUser == long.Parse(userId));

                    foreach (var address in otherAddresses)
                    {
                        address.AdrsIsDefault = false;
                    }
                }

                // Lưu địa chỉ mới vào cơ sở dữ liệu
                _context.AddressLists.Add(newAddress);
                _context.SaveChanges();

                // Chuyển hướng đến trang Index của Address
                return RedirectToAction("Index", "Address");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và hiển thị thông báo lỗi
                TempData["ErrorMessage"] = $"Đã có lỗi xảy ra: {ex.Message}";
                return View(); // Hoặc trả về một View lỗi
            }
        }
    }
}
