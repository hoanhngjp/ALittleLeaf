namespace ALittleLeaf.ViewModels
{
    public class OrderStatisticsViewModel
    {
        public int TotalOrders { get; set; } // Tổng số đơn hàng
        public int ConfirmedOrders { get; set; } // Số đơn hàng đã xác nhận
        public int PendingShippingOrders { get; set; } // Số đơn hàng chờ giao
        public int TotalRevenue { get; set; } // Tổng doanh thu
        public List<BillViewModel> RecentOrders { get; set; } // Danh sách đơn hàng gần đây
        // Dữ liệu cho biểu đồ
        public List<string> RevenueChartLabels { get; set; }
        public List<int> RevenueChartData { get; set; }

        // Dùng để lưu trữ giá trị của bộ lọc ngày
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        // Biểu đồ Trạng thái Giao hàng
        public List<string> ShippingStatusLabels { get; set; }
        public List<int> ShippingStatusData { get; set; }
        public List<string> ShippingStatusColors { get; set; }

        // Biểu đồ Trạng thái Thanh toán
        public List<string> PaymentStatusLabels { get; set; }
        public List<int> PaymentStatusData { get; set; }
        public List<string> PaymentStatusColors { get; set; }
    }
}
