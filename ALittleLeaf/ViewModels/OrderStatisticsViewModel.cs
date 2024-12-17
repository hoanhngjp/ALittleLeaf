namespace ALittleLeaf.ViewModels
{
    public class OrderStatisticsViewModel
    {
        public int TotalOrders { get; set; } // Tổng số đơn hàng
        public int ConfirmedOrders { get; set; } // Số đơn hàng đã xác nhận
        public int PendingShippingOrders { get; set; } // Số đơn hàng chờ giao
        public int TotalRevenue { get; set; } // Tổng doanh thu
        public List<BillViewModel> RecentOrders { get; set; } // Danh sách đơn hàng gần đây
    }
}
