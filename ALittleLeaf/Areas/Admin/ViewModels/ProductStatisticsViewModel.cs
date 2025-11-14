using ALittleLeaf.Areas.Admin.ViewModels;

namespace ALittleLeaf.ViewModels
{
    public class ProductStatisticsViewModel
    {
        public int TotalProducts { get; set; } // Tổng số sản phẩm
        public int TotalSoldProducts { get; set; } // Tổng số sản phẩm đã bán
        public int TotalStock { get; set; } // Tổng số sản phẩm còn tồn kho
        public List<TopProductViewModel> TopSellingProducts { get; set; } // Top sản phẩm bán chạy
        public List<LowStockProductViewModel> LowStockProducts { get; set; }
        public List<string> CategorySalesLabels { get; set; }
        public List<int> CategorySalesData { get; set; }
        public List<string> CategorySalesColors { get; set; }
    }
}
