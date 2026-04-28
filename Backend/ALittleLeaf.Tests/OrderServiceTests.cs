using ALittleLeaf.Api.DTOs.Cart;
using ALittleLeaf.Api.DTOs.Order;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Api.Services.Shipping;
using ALittleLeaf.Api.Services.VNPay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ALittleLeaf.Tests;

public class OrderServiceTests
{
    // ── Fixture helpers ───────────────────────────────────────────────────────

    private static (OrderService svc,
                    Mock<IOrderRepository> repoMock,
                    Mock<ICartService> cartMock)
        BuildSut()
    {
        var repoMock  = new Mock<IOrderRepository>();
        var cartMock  = new Mock<ICartService>();
        var vnpayMock = new Mock<IVnPayService>();
        var ghnMock   = new Mock<IGhnService>();
        var logger    = NullLogger<OrderService>.Instance;

        var svc = new OrderService(repoMock.Object, cartMock.Object,
                                   vnpayMock.Object, ghnMock.Object, logger);

        return (svc, repoMock, cartMock);
    }

    private static CartDto MakeCart(params (int productId, int price, int qty)[] items) =>
        new()
        {
            CartId = 1,
            Items  = items.Select(i => new CartItemDto
            {
                ProductId    = i.productId,
                ProductName  = $"Product {i.productId}",
                ProductPrice = i.price,
                Quantity     = i.qty,
            }).ToList(),
        };

    private static AddressList MakeAddress(int id = 1, long userId = 42) =>
        new()
        {
            AdrsId       = id,
            IdUser       = userId,
            AdrsFullname = "Nguyễn Văn A",
            AdrsPhone    = "0901234567",
            AdrsAddress  = "123 Lê Lợi",
        };

    private static Bill MakeSavedBill(int billId, Bill source)
    {
        source.BillId       = billId;
        source.BillDetails  = [];
        source.IdAdrsNavigation = MakeAddress();
        return source;
    }

    /// <summary>
    /// Sets up GetProductByIdAsync and SaveChangesAsync on the repo mock for every
    /// product referenced by the cart. Required by the Phase 16 stock deduction path.
    /// </summary>
    private static void SetupProductsAndSave(Mock<IOrderRepository> repo, CartDto cart)
    {
        foreach (var item in cart.Items)
        {
            var product = new Product
            {
                ProductId       = item.ProductId,
                ProductName     = item.ProductName ?? $"Product {item.ProductId}",
                QuantityInStock = 999,          // plenty of stock for non-stock tests
                ProductPrice    = item.ProductPrice,
                RowVersion      = [0, 0, 0, 0, 0, 0, 0, 1],
            };
            repo.Setup(r => r.GetProductByIdAsync(item.ProductId)).ReturnsAsync(product);
        }
        repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    // ── TotalAmount = cartSubtotal + ShippingFee ──────────────────────────────

    [Fact]
    public async Task CreateOrderAsync_TotalAmount_EqualsSubtotalPlusShippingFee()
    {
        var (svc, repo, cart) = BuildSut();

        const long   userId      = 42;
        const int    shippingFee = 35_000;
        var          address     = MakeAddress(userId: userId);
        var          cartDto     = MakeCart((1, 150_000, 2), (2, 80_000, 1));
        // cart subtotal = 150_000*2 + 80_000*1 = 380_000

        cart.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(cartDto);
        cart.Setup(c => c.ClearCartAsync(userId)).ReturnsAsync(new CartDto());
        SetupProductsAndSave(repo, cartDto);

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        Bill? capturedBill = null;
        repo.Setup(r => r.CreateBillAsync(It.IsAny<Bill>()))
            .Callback<Bill>(b => capturedBill = b)
            .ReturnsAsync((Bill b) => MakeSavedBill(1001, b));

        repo.Setup(r => r.AddBillDetailsAsync(It.IsAny<IEnumerable<BillDetail>>()))
            .Returns(Task.CompletedTask);

        repo.Setup(r => r.GetBillByIdAsync(1001))
            .ReturnsAsync(() => capturedBill!);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = shippingFee,
        };

        var result = await svc.CreateOrderAsync(userId, dto);

        Assert.Equal(380_000 + shippingFee, capturedBill!.TotalAmount);
        Assert.Equal(shippingFee,           capturedBill.ShippingFee);
    }

    [Fact]
    public async Task CreateOrderAsync_TotalAmount_WithZeroShippingFee_EqualsSubtotal()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);
        var        cartDto = MakeCart((1, 100_000, 3));
        // subtotal = 300_000

        cart.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(cartDto);
        cart.Setup(c => c.ClearCartAsync(userId)).ReturnsAsync(new CartDto());
        SetupProductsAndSave(repo, cartDto);

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        Bill? capturedBill = null;
        repo.Setup(r => r.CreateBillAsync(It.IsAny<Bill>()))
            .Callback<Bill>(b => capturedBill = b)
            .ReturnsAsync((Bill b) => MakeSavedBill(1002, b));

        repo.Setup(r => r.AddBillDetailsAsync(It.IsAny<IEnumerable<BillDetail>>()))
            .Returns(Task.CompletedTask);

        repo.Setup(r => r.GetBillByIdAsync(1002))
            .ReturnsAsync(() => capturedBill!);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = 0,
        };

        await svc.CreateOrderAsync(userId, dto);

        Assert.Equal(300_000, capturedBill!.TotalAmount);
        Assert.Equal(0,       capturedBill.ShippingFee);
    }

    // ── OrderStatus defaults to PENDING ──────────────────────────────────────

    [Fact]
    public async Task CreateOrderAsync_OrderStatus_DefaultsToPending()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);
        var        cartDto = MakeCart((1, 50_000, 1));

        cart.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(cartDto);
        cart.Setup(c => c.ClearCartAsync(userId)).ReturnsAsync(new CartDto());
        SetupProductsAndSave(repo, cartDto);

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        Bill? capturedBill = null;
        repo.Setup(r => r.CreateBillAsync(It.IsAny<Bill>()))
            .Callback<Bill>(b => capturedBill = b)
            .ReturnsAsync((Bill b) => MakeSavedBill(1003, b));

        repo.Setup(r => r.AddBillDetailsAsync(It.IsAny<IEnumerable<BillDetail>>()))
            .Returns(Task.CompletedTask);

        repo.Setup(r => r.GetBillByIdAsync(1003))
            .ReturnsAsync(() => capturedBill!);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = 0,
        };

        await svc.CreateOrderAsync(userId, dto);

        Assert.Equal("PENDING", capturedBill!.OrderStatus);
        Assert.False(capturedBill.IsConfirmed);
    }

    // ── COD order starts with pending_payment, not paid ───────────────────────

    [Fact]
    public async Task CreateOrderAsync_CodOrder_PaymentStatus_IsPending()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);
        var        cartDto = MakeCart((1, 50_000, 1));

        cart.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(cartDto);
        cart.Setup(c => c.ClearCartAsync(userId)).ReturnsAsync(new CartDto());
        SetupProductsAndSave(repo, cartDto);

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        Bill? capturedBill = null;
        repo.Setup(r => r.CreateBillAsync(It.IsAny<Bill>()))
            .Callback<Bill>(b => capturedBill = b)
            .ReturnsAsync((Bill b) => MakeSavedBill(1004, b));

        repo.Setup(r => r.AddBillDetailsAsync(It.IsAny<IEnumerable<BillDetail>>()))
            .Returns(Task.CompletedTask);

        repo.Setup(r => r.GetBillByIdAsync(1004))
            .ReturnsAsync(() => capturedBill!);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = 0,
        };

        await svc.CreateOrderAsync(userId, dto);

        Assert.Equal("pending", capturedBill!.PaymentStatus);
    }

    // ── VNPay order starts with pending_vnpay ────────────────────────────────

    [Fact]
    public async Task CreateOrderAsync_VnpayOrder_PaymentStatus_IsPendingVnpay()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);
        var        cartDto = MakeCart((1, 50_000, 1));

        cart.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(cartDto);
        cart.Setup(c => c.ClearCartAsync(userId)).ReturnsAsync(new CartDto());
        SetupProductsAndSave(repo, cartDto);

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        Bill? capturedBill = null;
        repo.Setup(r => r.CreateBillAsync(It.IsAny<Bill>()))
            .Callback<Bill>(b => capturedBill = b)
            .ReturnsAsync((Bill b) => MakeSavedBill(1005, b));

        repo.Setup(r => r.AddBillDetailsAsync(It.IsAny<IEnumerable<BillDetail>>()))
            .Returns(Task.CompletedTask);

        repo.Setup(r => r.GetBillByIdAsync(1005))
            .ReturnsAsync(() => capturedBill!);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "VNPAY",
            ShippingFee   = 0,
        };

        await svc.CreateOrderAsync(userId, dto);

        Assert.Equal("pending_vnpay", capturedBill!.PaymentStatus);
    }

    // ── Empty cart is rejected ────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrderAsync_EmptyCart_ThrowsInvalidOperationException()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);

        cart.Setup(c => c.GetCartAsync(userId))
            .ReturnsAsync(new CartDto { CartId = 1, Items = [] });

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = 0,
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.CreateOrderAsync(userId, dto));
    }

    // ── Stock is deducted at order creation (Phase 16) ───────────────────────

    [Fact]
    public async Task CreateOrderAsync_DeductsStock_BeforePersisting()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);
        var        product = new Product
        {
            ProductId       = 7,
            ProductName     = "Cây Sen Đá",
            QuantityInStock = 10,
            ProductPrice    = 50_000,
            RowVersion      = [0, 0, 0, 0, 0, 0, 0, 1],
        };

        cart.Setup(c => c.GetCartAsync(userId))
            .ReturnsAsync(MakeCart((7, 50_000, 3)));   // order 3 units

        cart.Setup(c => c.ClearCartAsync(userId))
            .ReturnsAsync(new CartDto());

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        repo.Setup(r => r.GetProductByIdAsync(7))
            .ReturnsAsync(product);

        // SaveChangesAsync succeeds — capture when it's called to inspect stock
        int stockAfterSave = -1;
        repo.Setup(r => r.SaveChangesAsync())
            .Callback(() => stockAfterSave = product.QuantityInStock)
            .Returns(Task.CompletedTask);

        Bill? capturedBill = null;
        repo.Setup(r => r.CreateBillAsync(It.IsAny<Bill>()))
            .Callback<Bill>(b => capturedBill = b)
            .ReturnsAsync((Bill b) => MakeSavedBill(2001, b));

        repo.Setup(r => r.AddBillDetailsAsync(It.IsAny<IEnumerable<BillDetail>>()))
            .Returns(Task.CompletedTask);

        repo.Setup(r => r.GetBillByIdAsync(2001))
            .ReturnsAsync(() => capturedBill!);

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = 0,
        };

        await svc.CreateOrderAsync(userId, dto);

        // Stock must have been decremented by 3 before SaveChangesAsync was called
        Assert.Equal(7, stockAfterSave);   // 10 - 3 = 7
    }

    // ── Concurrent buyer triggers friendly concurrency error (Phase 16) ───────

    [Fact]
    public async Task CreateOrderAsync_ConcurrencyConflict_ThrowsFriendlyMessage()
    {
        var (svc, repo, cart) = BuildSut();

        const long userId  = 42;
        var        address = MakeAddress(userId: userId);
        var        product = new Product
        {
            ProductId       = 8,
            ProductName     = "Cây Kim Tiền",
            QuantityInStock = 5,
            ProductPrice    = 120_000,
            RowVersion      = [0, 0, 0, 0, 0, 0, 0, 1],
        };

        cart.Setup(c => c.GetCartAsync(userId))
            .ReturnsAsync(MakeCart((8, 120_000, 2)));

        repo.Setup(r => r.GetAddressByIdAsync(address.AdrsId))
            .ReturnsAsync(address);

        repo.Setup(r => r.GetProductByIdAsync(8))
            .ReturnsAsync(product);

        // Simulate another request committing first — RowVersion mismatch
        repo.Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateConcurrencyException("Simulated concurrency conflict"));

        var dto = new CreateOrderDto
        {
            AddressId     = address.AdrsId,
            PaymentMethod = "COD",
            ShippingFee   = 0,
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.CreateOrderAsync(userId, dto));

        Assert.Contains("Sản phẩm vừa hết hàng", ex.Message);
    }
}
