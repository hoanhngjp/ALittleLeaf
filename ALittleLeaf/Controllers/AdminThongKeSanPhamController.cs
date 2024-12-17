using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminThongKeSanPhamController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminThongKeSanPhamController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            // Tổng số sản phẩm
            int totalProducts = _context.Products.Count();

            // Tổng số sản phẩm đã bán (tính tổng Quantity từ BillDetails)
            int totalSoldProducts = _context.BillDetails.Sum(bd => bd.Quantity);

            // Tổng số sản phẩm còn tồn kho
            int totalStock = _context.Products.Sum(p => p.QuantityInStock);

            // Lấy top 10 sản phẩm bán chạy nhất
            var topSellingProducts = _context.BillDetails
                .GroupBy(bd => bd.IdProduct)
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key,
                    ProductName = _context.Products.FirstOrDefault(p => p.ProductId == g.Key).ProductName,
                    QuantitySold = g.Sum(bd => bd.Quantity),
                    Stock = _context.Products.FirstOrDefault(p => p.ProductId == g.Key).QuantityInStock
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(10)
                .ToList();

            // Gán dữ liệu vào ViewModel
            var model = new ProductStatisticsViewModel
            {
                TotalProducts = totalProducts,
                TotalSoldProducts = totalSoldProducts,
                TotalStock = totalStock,
                TopSellingProducts = topSellingProducts
            };

            return View(model);
        }
    }
}
