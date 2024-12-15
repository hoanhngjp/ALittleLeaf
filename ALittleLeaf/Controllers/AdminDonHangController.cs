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
                    ShippingStatus = b.ShippingStatus,
                    Note = b.Note,
                    UpdatedAt = b.UpdatedAt,
                }).ToList();

            int totalItems = bills.Count();

            var pagination = new Paginate(totalItems, page, pageSize);

            ViewBag.Pagination = pagination;


            return View(result);
        }

    }
}
