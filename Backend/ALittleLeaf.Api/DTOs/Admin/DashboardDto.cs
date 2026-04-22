namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>Aggregated stats returned by GET /api/admin/dashboard.</summary>
    public class DashboardDto
    {
        public long  TotalRevenue       { get; set; }
        public int   TotalOrders        { get; set; }
        public int   PendingOrders      { get; set; }
        public int   TotalUsers         { get; set; }
        public int   TotalProducts      { get; set; }
        public IEnumerable<LowStockProductDto>  LowStockProducts  { get; set; } = [];
        public IEnumerable<TopSellingProductDto> TopSellingProducts { get; set; } = [];
        public IEnumerable<RevenueByMonthDto>       RevenueByMonth      { get; set; } = [];
        public IEnumerable<RevenueByCategoryDto>    RevenueByCategory   { get; set; } = [];
    }

    public class LowStockProductDto
    {
        public int    ProductId       { get; set; }
        public string ProductName     { get; set; } = string.Empty;
        public int    QuantityInStock { get; set; }
        public string? PrimaryImage   { get; set; }
    }

    public class TopSellingProductDto
    {
        public int    ProductId    { get; set; }
        public string ProductName  { get; set; } = string.Empty;
        public int    TotalSold    { get; set; }
        public long   TotalRevenue { get; set; }
        public string? PrimaryImage { get; set; }
    }

    public class RevenueByMonthDto
    {
        public int  Year    { get; set; }
        public int  Month   { get; set; }
        public long Revenue { get; set; }
    }

    public class RevenueByCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public long   Revenue      { get; set; }
    }
}
