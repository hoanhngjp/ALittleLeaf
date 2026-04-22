using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Cart
{
    public interface ICartRepository
    {
        /// <summary>
        /// Returns the cart (with items + product images) for <paramref name="userId"/>,
        /// or null when no cart exists yet.
        /// </summary>
        Task<Models.Cart?> GetByUserIdAsync(long userId);

        /// <summary>Creates and persists a new empty cart for the user.</summary>
        Task<Models.Cart> CreateAsync(long userId);

        /// <summary>Persists changes made to cart or its items (e.g. updated quantities).</summary>
        Task SaveChangesAsync();

        /// <summary>Adds a new CartItem row to the cart.</summary>
        Task AddItemAsync(CartItem item);

        /// <summary>Removes a CartItem row from the cart.</summary>
        void RemoveItem(CartItem item);

        /// <summary>Removes all CartItem rows belonging to the cart.</summary>
        void RemoveAllItems(IEnumerable<CartItem> items);
    }
}
