namespace ALittleLeaf.ViewModels
{
    public class AdminViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalBills { get; set; }
        public int TotalProducts { get; set; }
        // Biểu đồ Doanh thu
        public List<string> RevenueChartLabels { get; set; }
        public List<int> RevenueChartData { get; set; }
        // Biểu đồ Trạng thái đơn hàng
        public List<string> OrderStatusLabels { get; set; }
        public List<int> OrderStatusData { get; set; }
        public List<string> OrderStatusColors { get; set; }
        // Biểu đồ Phương thức thanh toán
        public List<string> PaymentMethodLabels { get; set; }
        public List<int> PaymentMethodData { get; set; }
        public List<string> PaymentMethodColors { get; set; }
    }
}
