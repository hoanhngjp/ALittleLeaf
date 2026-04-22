using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public CartRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public async Task<Models.Cart?> GetByUserIdAsync(long userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Models.Cart> CreateAsync(long userId)
        {
            var cart = new Models.Cart
            {
                UserId    = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public void RemoveItem(CartItem item)
        {
            _context.CartItems.Remove(item);
        }

        public void RemoveAllItems(IEnumerable<CartItem> items)
        {
            _context.CartItems.RemoveRange(items);
        }
    }
}
