using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public OrdersController(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        private IActionResult CheckAdminSession()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Login", "Account", new { Area = "Admin" });
            }
            return null;
        }

        // GET: /Admin/Orders
        // GET: /Admin/Orders/Index
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            var sessionError = CheckAdminSession();
            if (sessionError != null) return sessionError;

            int pageSize = 10;

            var bills = _context.Bills
                .Include(b => b.IdUserNavigation)
                .Include(b => b.IdAdrsNavigation)
                .AsQueryable();

            var result = bills
                .OrderByDescending(b => b.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname,
                    CustomerPhone = b.IdAdrsNavigation.AdrsPhone,
                    ShippingAddress = b.IdAdrsNavigation.AdrsAddress,
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

            // Areas/Admin/Views/Orders/Index.cshtml
            return View(result);
        }

        // GET: /Admin/Orders/Search
        [HttpGet]
        public IActionResult Search(DateOnly? FromDate, DateOnly? ToDate, string? PaymentStatus, bool? IsConfirmed, string? ShippingStatus, string? SearchType, string? SearchKey, int page = 1, int pageSize = 10)
        {
            var sessionError = CheckAdminSession();
            if (sessionError != null) return Unauthorized();

            var billQuery = _context.Bills
                .Include(b => b.IdUserNavigation)
                .Include(b => b.IdAdrsNavigation)
                .AsQueryable();

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
            if (FromDate.HasValue)
            {
                billQuery = billQuery.Where(b => b.DateCreated >= FromDate.Value);
            }
            if (ToDate.HasValue)
            {
                billQuery = billQuery.Where(b => b.DateCreated <= ToDate.Value);
            }
            if (!string.IsNullOrEmpty(PaymentStatus))
            {
                billQuery = billQuery.Where(b => b.PaymentStatus.Equals(PaymentStatus));
            }
            if (IsConfirmed.HasValue)
            {
                billQuery = billQuery.Where(b => b.IsConfirmed == IsConfirmed.Value);
            }
            if (!string.IsNullOrEmpty(ShippingStatus))
            {
                billQuery = billQuery.Where(b => b.ShippingStatus.Equals(ShippingStatus));
            }

            int totalItems = billQuery.Count();
            var bills = billQuery
                .OrderByDescending(b => b.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname,
                    CustomerPhone = b.IdAdrsNavigation.AdrsPhone,
                    ShippingAddress = b.IdAdrsNavigation.AdrsAddress,
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    IsConfirmed = b.IsConfirmed,
                    PaymentStatus = b.PaymentStatus,
                    ShippingStatus = b.ShippingStatus,
                    Note = b.Note,
                    UpdatedAt = b.UpdatedAt,
                }).ToList();

            if (!bills.Any())
            {
                ViewBag.Message = "Không tìm thấy đơn hàng nào phù hợp.";
            }

            var model = new BillSearchViewModel
            {
                Bills = bills,
                Pagination = new Paginate(totalItems, page, pageSize)
            };

            // Areas/Admin/Views/Shared/_AdminSearchBillResult.cshtml
            return PartialView("_AdminSearchBillResult", model);
        }


        // GET: /Admin/Orders/Statistics
        [HttpGet]
        public async Task<IActionResult> Statistics(DateOnly? fromDate, DateOnly? toDate)
        {
            var sessionError = CheckAdminSession();
            if (sessionError != null) return sessionError;

            // 1. Xử lý ngày tháng
            var today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly startDate = fromDate ?? today.AddDays(-29); // Mặc định 30 ngày qua
            DateOnly endDate = toDate ?? today;

            // 2. Lấy base query (các hóa đơn trong khoảng ngày)
            var billsInDateRange = _context.Bills
                .Where(b => b.DateCreated >= startDate && b.DateCreated <= endDate);

            // 3. Tính 4 Box KPI
            int totalOrders = await billsInDateRange.CountAsync();
            int totalRevenue = 0;
            int confirmedOrders = 0;
            int pendingShippingOrders = 0;

            if (totalOrders > 0)
            {
                // Chỉ tính khi có đơn, tránh lỗi SumAsync() trên tập rỗng
                totalRevenue = await billsInDateRange.SumAsync(b => b.TotalAmount);
                confirmedOrders = await billsInDateRange.CountAsync(b => b.IsConfirmed == true);
                pendingShippingOrders = await billsInDateRange.CountAsync(b => b.ShippingStatus == "not_fulfilled");
            }

            // 4. Lấy 10 đơn hàng gần đây (cho bảng)
            var recentOrders = await _context.Bills
                .Include(b => b.IdUserNavigation)
                .OrderByDescending(b => b.DateCreated)
                .Take(10)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname,
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    IsConfirmed = b.IsConfirmed,
                    ShippingStatus = b.ShippingStatus
                })
                .ToListAsync();

            // 5. Lấy dữ liệu biểu đồ đường (Doanh thu)
            var dailyRevenue = await billsInDateRange
                .Where(b => b.IsConfirmed == true) // Chỉ tính doanh thu đơn đã xác nhận
                .GroupBy(b => b.DateCreated)
                .Select(g => new { Day = g.Key, Total = g.Sum(b => b.TotalAmount) })
                .ToDictionaryAsync(r => r.Day, r => r.Total);

            var chartLabels = new List<string>();
            var chartData = new List<int>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                chartLabels.Add(date.ToString("dd/MM"));
                chartData.Add(dailyRevenue.ContainsKey(date) ? dailyRevenue[date] : 0);
            }

            // 6. THÊM MỚI: Lấy dữ liệu Biểu đồ Trạng thái Giao hàng
            var shippingQuery = await billsInDateRange
                .GroupBy(b => b.ShippingStatus)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToListAsync();

            var shippingLabels = new List<string>();
            var shippingData = new List<int>();
            var shippingColors = new List<string>();
            foreach (var item in shippingQuery)
            {
                switch (item.Key?.ToLower())
                {
                    case "fulfilled":
                        shippingLabels.Add("Đã giao hàng");
                        shippingData.Add(item.Count);
                        shippingColors.Add("#28a745"); // Xanh lá
                        break;
                    case "not_fulfilled":
                        shippingLabels.Add("Chưa giao hàng");
                        shippingData.Add(item.Count);
                        shippingColors.Add("#ffc107"); // Vàng
                        break;
                    default:
                        shippingLabels.Add(item.Key ?? "Không rõ");
                        shippingData.Add(item.Count);
                        shippingColors.Add("#6c757d"); // Xám
                        break;
                }
            }

            // 7. Lấy dữ liệu Biểu đồ Trạng thái Thanh toán
            var paymentQuery = await billsInDateRange
                .GroupBy(b => b.PaymentStatus)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToListAsync();

            var paymentLabels = new List<string>();
            var paymentData = new List<int>();
            var paymentColors = new List<string>();
            foreach (var item in paymentQuery)
            {
                // Giả sử bạn có 2 trạng thái "paid" và "unpaid"
                switch (item.Key?.ToLower())
                {
                    case "paid":
                        paymentLabels.Add("Đã thanh toán");
                        paymentData.Add(item.Count);
                        paymentColors.Add("#007bff"); // Xanh dương
                        break;
                    case "pending":
                        paymentLabels.Add("Chưa thanh toán");
                        paymentData.Add(item.Count);
                        paymentColors.Add("#dc3545"); // Đỏ
                        break;
                    default:
                        paymentLabels.Add(item.Key ?? "Không rõ");
                        paymentData.Add(item.Count);
                        paymentColors.Add("#6c757d"); // Xám
                        break;
                }
            }

            // 8. Tạo ViewModel
            var model = new OrderStatisticsViewModel
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                ConfirmedOrders = confirmedOrders,
                PendingShippingOrders = pendingShippingOrders,
                RecentOrders = recentOrders,
                RevenueChartLabels = chartLabels,
                RevenueChartData = chartData,
                FromDate = startDate,
                ToDate = endDate,
                ShippingStatusLabels = shippingLabels,
                ShippingStatusData = shippingData,
                ShippingStatusColors = shippingColors,
                PaymentStatusLabels = paymentLabels,
                PaymentStatusData = paymentData,
                PaymentStatusColors = paymentColors
            };

            return View(model);
        }

        // GET: /Admin/Orders/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var sessionError = CheckAdminSession();
            if (sessionError != null) return sessionError;

            var bill = _context.Bills
                .Include(b => b.IdUserNavigation)
                .Include(b => b.IdAdrsNavigation)
                .Where(b => b.BillId == id)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname,
                    CustomerPhone = b.IdAdrsNavigation.AdrsPhone,
                    ShippingAddress = b.IdAdrsNavigation.AdrsAddress,
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    PaymentStatus = b.PaymentStatus,
                    IsConfirmed = b.IsConfirmed,
                    Note = b.Note,
                    ShippingStatus = b.ShippingStatus,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefault();

            if (bill == null)
            {
                return NotFound();
            }

            var orderDetails = (from bd in _context.BillDetails
                                join p in _context.Products on bd.IdProduct equals p.ProductId
                                join pi in _context.ProductImages on p.ProductId equals pi.IdProduct
                                where bd.IdBill == id && pi.IsPrimary == true
                                select new OrderDetailViewModel
                                {
                                    BillDetailId = bd.BillDetailId,
                                    IdBill = bd.IdBill,
                                    IdProduct = bd.IdProduct,
                                    IdCategory = p.IdCategory,
                                    Quantity = bd.Quantity,
                                    UnitPrice = bd.UnitPrice,
                                    TotalPrice = bd.TotalPrice,
                                    ProductName = p.ProductName,
                                    ProductImg = pi.ImgName
                                }).ToList();

            var model = new AdminBillDetailViewModel
            {
                Bill = bill,
                OrderDetails = orderDetails
            };

            // Areas/Admin/Views/Orders/Details.cshtml
            return View(model);
        }

        // POST: /Admin/Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int BillId, bool IsConfirmed, string PaymentStatus, string ShippingStatus)
        {
            var sessionError = CheckAdminSession();
            if (sessionError != null) return sessionError;

            var bill = _context.Bills.FirstOrDefault(b => b.BillId == BillId);

            if (bill != null)
            {
                bill.IsConfirmed = IsConfirmed;
                bill.PaymentStatus = PaymentStatus;
                bill.ShippingStatus = ShippingStatus;
                bill.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }

            // SỬA: Chuyển hướng về trang Details
            return RedirectToAction("Details", new { id = BillId });
        }
    }
}
