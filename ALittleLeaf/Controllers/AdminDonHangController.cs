using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{

    public class AdminDonHangController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminDonHangController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            int pageSize = 10;

            var bills = _context.Bills.AsQueryable();

            var result = bills
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname, // Tên khách hàng từ bảng User
                    CustomerPhone = b.IdAdrsNavigation.AdrsPhone,  // Số điện thoại từ bảng User
                    ShippingAddress = b.IdAdrsNavigation.AdrsAddress, // Địa chỉ giao hàng từ AddressList
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    PaymentStatus = b.PaymentStatus,
                    IsConfirmed = b.IsConfirmed,
                    ShippingStatus = b.ShippingStatus,
                    Note = b.Note,
                    UpdatedAt = b.UpdatedAt,
                }).ToList();

            int totalItems = bills.Count();

            var pagination = new Paginate(totalItems, page, pageSize);

            ViewBag.Pagination = pagination;

            return View(result);
        }

        [HttpGet]
        public IActionResult SearchBill(DateOnly? FromDate, DateOnly? ToDate, string? PaymentStatus, bool? IsConfirmed, string? ShippingStatus, string? SearchType, string? SearchKey, int page = 1, int pageSize = 10)
        {
            var billQuery = _context.Bills
                .AsQueryable();

            // Áp dụng logic tìm kiếm
            if (!string.IsNullOrEmpty(SearchKey))
            {
                switch (SearchType)
                {
                    case "SearchByBillID":
                        billQuery = billQuery.Where(b => b.BillId.ToString().Contains(SearchKey));
                        break;
                    case "SearchByBillCustomerName":
                        billQuery = billQuery.Where(b => b.IdUserNavigation.UserFullname.Contains(SearchKey));
                        break;
                }
            }
            // Lọc theo FromDate và ToDate
            if (FromDate.HasValue)
            {
                billQuery = billQuery.Where(b => b.DateCreated >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                billQuery = billQuery.Where(b => b.DateCreated <= ToDate.Value);
            }

            // Lọc theo PaymentStatus
            if (!string.IsNullOrEmpty(PaymentStatus))
            {
                billQuery = billQuery.Where(b => b.PaymentStatus.Equals(PaymentStatus));
            }

            // Lọc theo IsConfirmed
            if (IsConfirmed.HasValue)
            {
                billQuery = billQuery.Where(b => b.IsConfirmed == IsConfirmed.Value);
            }

            // Lọc theo ShippingStatus
            if (!string.IsNullOrEmpty(ShippingStatus))
            {
                billQuery = billQuery.Where(b => b.ShippingStatus.Equals(ShippingStatus));
            }

            // Tổng số bản ghi
            int totalItems = billQuery.Count();

            // Lấy danh sách các user thuộc trang hiện tại
            var bills = billQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname, // Tên khách hàng từ bảng User
                    CustomerPhone = b.IdAdrsNavigation.AdrsPhone,  // Số điện thoại từ bảng User
                    ShippingAddress = b.IdAdrsNavigation.AdrsAddress, // Địa chỉ giao hàng từ AddressList
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    IsConfirmed = b.IsConfirmed,
                    PaymentStatus = b.PaymentStatus,
                    ShippingStatus = b.ShippingStatus,
                    Note = b.Note,
                    UpdatedAt = b.UpdatedAt,
                }).ToList();

            // Nếu không có kết quả, tạo ViewBag chứa thông báo
            if (!bills.Any())
            {
                ViewBag.Message = "Không tìm thấy đơn hàng nào phù hợp với từ khóa tìm kiếm.";
            }

            // Tạo ViewModel để truyền dữ liệu
            var model = new BillSearchViewModel
            {
                Bills = bills,
                Pagination = new Paginate(totalItems, page, pageSize)
            };
            return PartialView("_AdminSearchBillResult", model);
        }
    }
}
