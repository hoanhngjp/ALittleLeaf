using ALittleLeaf.Api.DTOs.Cart;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Cart;

namespace ALittleLeaf.Api.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;

        public CartService(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        // ── Get or create ─────────────────────────────────────────────────────

        public async Task<CartDto> GetCartAsync(long userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId)
                       ?? await _cartRepo.CreateAsync(userId);

            return MapToDto(cart);
        }

        // ── Add item ──────────────────────────────────────────────────────────

        public async Task<CartDto> AddItemAsync(long userId, int productId, int quantity)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId)
                       ?? await _cartRepo.CreateAsync(userId);

            // Increment quantity if product is already in the cart
            var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                await _cartRepo.AddItemAsync(new CartItem
                {
                    CartId    = cart.CartId,
                    ProductId = productId,
                    Quantity  = quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.SaveChangesAsync();

            // Reload to include product navigation for the response
            var updated = await _cartRepo.GetByUserIdAsync(userId);
            return MapToDto(updated!);
        }

        // ── Update item ───────────────────────────────────────────────────────

        public async Task<CartDto?> UpdateItemAsync(long userId, int productId, int quantity)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null) return null;

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null) return null;

            item.Quantity  = quantity;
            item.UpdatedAt = DateTime.UtcNow;
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepo.SaveChangesAsync();
            return MapToDto(cart);
        }

        // ── Remove item ───────────────────────────────────────────────────────

        public async Task<CartDto?> RemoveItemAsync(long userId, int productId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null) return null;

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null) return null;

            _cartRepo.RemoveItem(item);
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepo.SaveChangesAsync();
            return MapToDto(cart);
        }

        // ── Clear cart ────────────────────────────────────────────────────────

        public async Task<CartDto> ClearCartAsync(long userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId)
                       ?? await _cartRepo.CreateAsync(userId);

            _cartRepo.RemoveAllItems(cart.CartItems.ToList());
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepo.SaveChangesAsync();
            return MapToDto(cart);
        }

        // ── Mapping helper ────────────────────────────────────────────────────

        private static CartDto MapToDto(Models.Cart cart) => new()
        {
            CartId = cart.CartId,
            Items  = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId   = ci.CartItemId,
                ProductId    = ci.ProductId,
                ProductName  = ci.Product?.ProductName  ?? string.Empty,
                ProductPrice = ci.Product?.ProductPrice ?? 0,
                ProductImage = ci.Product?.ProductImages
                                          .FirstOrDefault(i => i.IsPrimary)?.ImgName,
                Quantity     = ci.Quantity
            })
        };
    }
}
