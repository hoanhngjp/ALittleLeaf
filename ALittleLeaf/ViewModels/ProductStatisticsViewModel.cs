namespace ALittleLeaf.ViewModels
{
    public class ProductStatisticsViewModel
    {
        public int TotalProducts { get; set; } // Tổng số sản phẩm
        public int TotalSoldProducts { get; set; } // Tổng số sản phẩm đã bán
        public int TotalStock { get; set; } // Tổng số sản phẩm còn tồn kho
        public List<TopProductViewModel> TopSellingProducts { get; set; } // Top sản phẩm bán chạy
    }
}
