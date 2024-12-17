using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminThongKeDonHang : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminThongKeDonHang(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            // Tổng số đơn hàng
            int totalOrders = _context.Bills.Count();

            // Tổng doanh thu (sum của TotalAmount)
            int totalRevenue = _context.Bills.Sum(b => b.TotalAmount);

            // Số đơn hàng đã xác nhận
            int confirmedOrders = _context.Bills.Count(b => b.IsConfirmed == true);

            // Số đơn hàng chờ giao (ví dụ ShippingStatus = "Chờ giao")
            int pendingShippingOrders = _context.Bills.Count(b => b.ShippingStatus == "not_fulfilled");

            // Lấy danh sách 10 đơn hàng gần đây
            var recentOrders = _context.Bills
                .OrderByDescending(b => b.DateCreated)
                .Take(10)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname,
                    IsConfirmed = b.IsConfirmed,
                    ShippingStatus = b.ShippingStatus,
                    TotalAmount = b.TotalAmount,
                    DateCreated = b.DateCreated
                })
                .ToList();

            // Đưa dữ liệu vào ViewModel
            var model = new OrderStatisticsViewModel
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                ConfirmedOrders = confirmedOrders,
                PendingShippingOrders = pendingShippingOrders,
                RecentOrders = recentOrders
            };

            return View(model);
        }
    }
}
