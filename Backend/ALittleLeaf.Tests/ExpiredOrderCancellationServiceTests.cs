using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Services.Background;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ALittleLeaf.Tests;

public class ExpiredOrderCancellationServiceTests
{
    // ── Fixture helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Builds the full mock scope chain required by BackgroundService:
    ///   IServiceScopeFactory → IServiceScope → IServiceProvider → IOrderRepository
    /// </summary>
    private static (ExpiredOrderCancellationService svc,
                    Mock<IOrderRepository> repoMock)
        BuildSut()
    {
        var repoMock         = new Mock<IOrderRepository>();
        var serviceProvider  = new Mock<IServiceProvider>();
        var serviceScope     = new Mock<IServiceScope>();
        var scopeFactory     = new Mock<IServiceScopeFactory>();

        serviceProvider
            .Setup(sp => sp.GetService(typeof(IOrderRepository)))
            .Returns(repoMock.Object);

        serviceScope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);

        scopeFactory
            .Setup(f => f.CreateScope())
            .Returns(serviceScope.Object);

        var logger = NullLogger<ExpiredOrderCancellationService>.Instance;
        var svc    = new ExpiredOrderCancellationService(scopeFactory.Object, logger);

        return (svc, repoMock);
    }

    private static Bill MakeExpiredBill(int billId, int productId, int qty, int currentStock)
    {
        var product = new Product
        {
            ProductId       = productId,
            ProductName     = $"Product {productId}",
            QuantityInStock = currentStock,
            ProductPrice    = 50_000,
            RowVersion      = [0, 0, 0, 0, 0, 0, 0, 1],
        };

        return new Bill
        {
            BillId        = billId,
            PaymentMethod = "VNPAY",
            PaymentStatus = "pending_vnpay",
            OrderStatus   = "PENDING",
            CreatedAtTime = DateTime.UtcNow.AddMinutes(-20),   // 20 min old → expired
            BillDetails   =
            [
                new BillDetail
                {
                    BillDetailId          = billId * 10,
                    IdBill                = billId,
                    IdProduct             = productId,
                    Quantity              = qty,
                    UnitPrice             = 50_000,
                    TotalPrice            = 50_000 * qty,
                    IdProductNavigation   = product,
                }
            ]
        };
    }

    // ── Scenario 6: Expired orders are cancelled and stock is restored ─────────

    [Fact]
    public async Task ExecuteAsync_ExpiredOrders_AreCancelledAndStockRestored()
    {
        var (svc, repo) = BuildSut();

        var expiredBill = MakeExpiredBill(billId: 555, productId: 10, qty: 3, currentStock: 7);
        var product     = expiredBill.BillDetails.First().IdProductNavigation!;

        repo.Setup(r => r.GetExpiredPendingVnpayOrdersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync([expiredBill]);

        repo.Setup(r => r.GetProductByIdAsync(10))
            .ReturnsAsync(product);

        repo.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Cancel after first iteration so the while-loop exits immediately
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        await svc.StartAsync(cts.Token);
        await Task.Delay(200);   // allow the first ExecuteAsync tick to complete
        await svc.StopAsync(CancellationToken.None);

        // Bill must be cancelled
        Assert.Equal("CANCELLED", expiredBill.OrderStatus);
        Assert.Equal("cancelled", expiredBill.PaymentStatus);

        // Stock must be restored: 7 + 3 = 10
        Assert.Equal(10, product.QuantityInStock);

        // SaveChangesAsync must have been called exactly once
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ── Scenario 7: Fresh orders within 15 minutes are not touched ─────────────

    [Fact]
    public async Task ExecuteAsync_FreshOrders_AreNotCancelled()
    {
        var (svc, repo) = BuildSut();

        // Repo returns empty list — no expired orders found
        repo.Setup(r => r.GetExpiredPendingVnpayOrdersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync([]);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        await svc.StartAsync(cts.Token);
        await Task.Delay(200);
        await svc.StopAsync(CancellationToken.None);

        // SaveChangesAsync must never be called when there is nothing to cancel
        repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
