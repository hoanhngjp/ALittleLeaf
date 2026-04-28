using ALittleLeaf.Api.Repositories.Order;

namespace ALittleLeaf.Api.Services.Background
{
    // WARNING: This job is not safe for multi-instance deployments.
    // If scaling out behind a load balancer, replace with a Distributed Lock
    // (e.g., Redis SETNX) or migrate to Hangfire with a single-instance scheduler.
    public class ExpiredOrderCancellationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredOrderCancellationService> _logger;

        private static readonly TimeSpan Interval     = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan OrderTimeout = TimeSpan.FromMinutes(15);

        public ExpiredOrderCancellationService(
            IServiceScopeFactory scopeFactory,
            ILogger<ExpiredOrderCancellationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger       = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExpiredOrderCancellationService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                await CancelExpiredOrdersAsync();
                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task CancelExpiredOrdersAsync()
        {
            using var scope      = _scopeFactory.CreateScope();
            var orderRepo        = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            var cutoff       = DateTime.UtcNow.Subtract(OrderTimeout);
            var expiredBills = await orderRepo.GetExpiredPendingVnpayOrdersAsync(cutoff);

            if (expiredBills.Count == 0) return;

            foreach (var bill in expiredBills)
            {
                // Restore reserved stock for each line item
                foreach (var detail in bill.BillDetails)
                {
                    var product = await orderRepo.GetProductByIdAsync(detail.IdProduct);
                    if (product != null)
                        product.QuantityInStock += detail.Quantity;
                }

                bill.OrderStatus   = "CANCELLED";
                bill.PaymentStatus = "cancelled";
                bill.UpdatedAt     = DateTime.UtcNow;
            }

            await orderRepo.SaveChangesAsync();
            _logger.LogInformation("Auto-cancelled {Count} expired VNPay orders.", expiredBills.Count);
        }
    }
}
