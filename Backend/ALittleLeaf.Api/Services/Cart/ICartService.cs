using ALittleLeaf.Api.DTOs.Cart;

namespace ALittleLeaf.Api.Services.Cart
{
    public interface ICartService
    {
        /// <summary>
        /// Returns the cart for <paramref name="userId"/>, creating an empty one if it
        /// does not exist yet (get-or-create semantics).
        /// </summary>
        Task<CartDto> GetCartAsync(long userId);

        /// <summary>
        /// Adds <paramref name="productId"/> to the cart with the given quantity.
        /// If the product is already in the cart its quantity is incremented instead.
        /// Returns the updated cart.
        /// </summary>
        Task<CartDto> AddItemAsync(long userId, int productId, int quantity);

        /// <summary>
        /// Sets the quantity of an existing cart item to the supplied value.
        /// Returns null when the item is not found in the cart.
        /// </summary>
        Task<CartDto?> UpdateItemAsync(long userId, int productId, int quantity);

        /// <summary>
        /// Removes a single product from the cart.
        /// Returns null when the item is not found in the cart.
        /// </summary>
        Task<CartDto?> RemoveItemAsync(long userId, int productId);

        /// <summary>Removes all items from the cart and returns the empty cart.</summary>
        Task<CartDto> ClearCartAsync(long userId);
    }
}
