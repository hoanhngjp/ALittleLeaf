using ALittleLeaf.Areas.Admin.Controllers;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly AlittleLeafDecorContext _context;

        public DashboardController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int totalUsers = await _context.Users.CountAsync();
            int totalProducts = await _context.Products.CountAsync();
            int totalBills = await _context.Bills.CountAsync();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var sevenDaysAgo = today.AddDays(-6);

            // 1. Lấy dữ liệu 7 ngày
            var dailyRevenue = await _context.Bills
                            .Where(b => b.DateCreated >= sevenDaysAgo && b.DateCreated <= today && b.IsConfirmed == true) // Chỉ lấy đơn đã xác nhận
                            .GroupBy(b => b.DateCreated)
                            .Select(g => new {
                                Day = g.Key,
                                Total = g.Sum(b => b.TotalAmount)
                            })
                            .ToDictionaryAsync(r => r.Day, r => r.Total);

            // 2. Chuẩn bị data cho Chart.js
            var chartLabels = new List<string>();
            var chartData = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                var date = sevenDaysAgo.AddDays(i);
                chartLabels.Add(date.ToString("dd/MM"));

                if (dailyRevenue.ContainsKey(date))
                {
                    chartData.Add(dailyRevenue[date]);
                }
                else
                {
                    chartData.Add(0);
                }
            }

            // LẤY DỮ LIỆU BIỂU ĐỒ TRẠNG THÁI ĐƠN
            var orderStatusQuery = await _context.Bills
                .GroupBy(b => b.ShippingStatus)
                .Select(g => new { StatusKey = g.Key, Count = g.Count() })
                .ToListAsync();

            var statusLabels = new List<string>();
            var statusCounts = new List<int>();
            var statusColors = new List<string>();

            foreach (var item in orderStatusQuery)
            {
                switch (item.StatusKey?.ToLower())
                {
                    case "fulfilled":
                        statusLabels.Add("Đã giao hàng");
                        statusCounts.Add(item.Count);
                        statusColors.Add("#28a745"); // Màu xanh lá
                        break;
                    case "not_fulfilled":
                        statusLabels.Add("Chưa giao hàng");
                        statusCounts.Add(item.Count);
                        statusColors.Add("#ffc107"); // Màu vàng
                        break;
                    default:
                        statusLabels.Add(item.StatusKey ?? "Không rõ");
                        statusCounts.Add(item.Count);
                        statusColors.Add("#6c757d"); // Màu xám
                        break;
                }
            }

            // LẤY DỮ LIỆU BIỂU ĐỒ PHƯƠNG THỨC THANH TOÁN
            var paymentMethodQuery = await _context.Bills
                .GroupBy(b => b.PaymentMethod)
                .Select(g => new { MethodKey = g.Key, Count = g.Count() })
                .ToListAsync();

            var paymentLabels = new List<string>();
            var paymentCounts = new List<int>();
            var paymentColors = new List<string>();

            foreach (var item in paymentMethodQuery)
            {
                switch (item.MethodKey?.ToUpper())
                {
                    case "COD":
                        paymentLabels.Add("Thu hộ (COD)");
                        paymentCounts.Add(item.Count);
                        paymentColors.Add("#fd7e14");
                        break;
                    case "ONLINE":
                        paymentLabels.Add("VNPAY");
                        paymentCounts.Add(item.Count);
                        paymentColors.Add("#007bff"); // Màu xanh dương
                        break;
                    // Thêm các PTTT khác nếu có
                    default:
                        paymentLabels.Add(item.MethodKey ?? "Không rõ");
                        paymentCounts.Add(item.Count);
                        paymentColors.Add("#6c757d"); // Màu xám
                        break;
                }
            }

            var adminViewModel = new AdminViewModel
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalBills = totalBills,
                RevenueChartLabels = chartLabels,
                RevenueChartData = chartData,
                OrderStatusLabels = statusLabels,
                OrderStatusData = statusCounts,
                OrderStatusColors = statusColors,
                PaymentMethodLabels = paymentLabels,
                PaymentMethodData = paymentCounts,
                PaymentMethodColors = paymentColors
            };                
                
            return View(adminViewModel);
        }
    }
}
