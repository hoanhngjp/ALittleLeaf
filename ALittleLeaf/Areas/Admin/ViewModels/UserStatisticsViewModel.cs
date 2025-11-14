namespace ALittleLeaf.Areas.Admin.ViewModels
{
    public class UserStatisticsViewModel
    {
        // 1. KPIs (Box số liệu)
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }

        // 2. Biểu đồ đường (Đăng ký mới)
        public List<string> RegistrationChartLabels { get; set; }
        public List<int> RegistrationChartData { get; set; }

        // 3. Biểu đồ tròn (Giới tính)
        public List<string> GenderLabels { get; set; }
        public List<int> GenderData { get; set; }
        public List<string> GenderColors { get; set; }

        // 4. Biểu đồ tròn (Trạng thái)
        public List<string> StatusLabels { get; set; }
        public List<int> StatusData { get; set; }
        public List<string> StatusColors { get; set; }

        // 5. Bộ lọc ngày
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }
}
