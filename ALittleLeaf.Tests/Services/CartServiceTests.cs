using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Cart;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Tests.Helpers;

namespace ALittleLeaf.Tests.Services
{
    /// <summary>
    /// Tests for the DB-backed CartService.
    /// Each test gets its own isolated InMemory DB via DbContextFactory.Create().
    /// No IHttpContextAccessor or ISession — userId is passed directly (as it is from JWT in production).
    /// </summary>
    public class CartServiceTests : IDisposable
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly CartService             _service;

        // Consistent test userId — products 1-3 are seeded by DbContextFactory
        private const long UserId = 42;

        public CartServiceTests()
        {
            _context = DbContextFactory.Create();
            _service = new CartService(new CartRepository(_context));
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        // ── AddItemAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task AddItemAsync_ShouldCreateNewCart_WhenCartDoesNotExist()
        {
            // Act — no cart exists for UserId=42 yet
            var cart = await _service.AddItemAsync(UserId, productId: 1, quantity: 2);

            // Assert
            Assert.NotNull(cart);
            Assert.Single(cart.Items);

            var item = cart.Items.First();
            Assert.Equal(1, item.ProductId);
            Assert.Equal(2, item.Quantity);

            // Verify Cart row was persisted
            var dbCart = _context.Carts.FirstOrDefault(c => c.UserId == UserId);
            Assert.NotNull(dbCart);
        }

        [Fact]
        public async Task AddItemAsync_ShouldIncreaseQuantity_WhenItemAlreadyExists()
        {
            // Arrange — add product 1 once
            await _service.AddItemAsync(UserId, productId: 1, quantity: 1);

            // Act — add it again
            var cart = await _service.AddItemAsync(UserId, productId: 1, quantity: 2);

            // Assert — quantity should be 1 + 2 = 3, still one line item
            Assert.Single(cart.Items);
            Assert.Equal(3, cart.Items.First().Quantity);
        }

        [Fact]
        public async Task AddItemAsync_ShouldAddSeparateLineItems_ForDifferentProducts()
        {
            await _service.AddItemAsync(UserId, productId: 1, quantity: 1);
            var cart = await _service.AddItemAsync(UserId, productId: 3, quantity: 2);

            Assert.Equal(2, cart.Items.Count());
            Assert.Contains(cart.Items, i => i.ProductId == 1 && i.Quantity == 1);
            Assert.Contains(cart.Items, i => i.ProductId == 3 && i.Quantity == 2);
        }

        // ── UpdateItemAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task UpdateItemAsync_ShouldChangeQuantity()
        {
            await _service.AddItemAsync(UserId, productId: 1, quantity: 1);

            var cart = await _service.UpdateItemAsync(UserId, productId: 1, quantity: 5);

            Assert.NotNull(cart);
            Assert.Equal(5, cart!.Items.First(i => i.ProductId == 1).Quantity);
        }

        [Fact]
        public async Task UpdateItemAsync_ShouldReturnNull_WhenCartDoesNotExist()
        {
            var result = await _service.UpdateItemAsync(userId: 999, productId: 1, quantity: 5);
            Assert.Null(result);
        }

        // ── RemoveItemAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task RemoveItemAsync_ShouldRemoveItemFromCart()
        {
            // Arrange — add two different products
            await _service.AddItemAsync(UserId, productId: 1, quantity: 2);
            await _service.AddItemAsync(UserId, productId: 3, quantity: 1);

            // Act — remove product 1
            var cart = await _service.RemoveItemAsync(UserId, productId: 1);

            // Assert
            Assert.NotNull(cart);
            Assert.Single(cart!.Items);
            Assert.All(cart.Items, i => Assert.NotEqual(1, i.ProductId));
        }

        [Fact]
        public async Task RemoveItemAsync_ShouldReturnNull_WhenItemNotInCart()
        {
            await _service.AddItemAsync(UserId, productId: 1, quantity: 1);

            var result = await _service.RemoveItemAsync(UserId, productId: 999);
            Assert.Null(result);
        }

        // ── ClearCartAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task ClearCartAsync_ShouldRemoveAllItems()
        {
            await _service.AddItemAsync(UserId, productId: 1, quantity: 1);
            await _service.AddItemAsync(UserId, productId: 2, quantity: 3);

            var cart = await _service.ClearCartAsync(UserId);

            Assert.NotNull(cart);
            Assert.Empty(cart.Items);

            // Verify CartItems are actually deleted from DB
            var dbCart  = _context.Carts.First(c => c.UserId == UserId);
            var dbItems = _context.CartItems.Where(ci => ci.CartId == dbCart.CartId).ToList();
            Assert.Empty(dbItems);
        }

        // ── GetCartAsync ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetCartAsync_ShouldCreateEmptyCart_WhenNoneExists()
        {
            var cart = await _service.GetCartAsync(UserId);

            Assert.NotNull(cart);
            Assert.Empty(cart.Items);
            Assert.True(cart.CartId > 0);
        }

        [Fact]
        public async Task GetCartAsync_ShouldReturnExistingCart()
        {
            await _service.AddItemAsync(UserId, productId: 1, quantity: 3);

            var cart = await _service.GetCartAsync(UserId);

            Assert.Single(cart.Items);
            Assert.Equal(3, cart.Items.First().Quantity);
        }
    }
}
