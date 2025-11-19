using ALittleLeaf.ViewModels;

namespace ALittleLeaf.Services.Cart
{
    public interface ICartService
    {
        List<CartItemViewModel> GetCartItems();

        int GetCartTotal();
        int GetCartItemCount();

        Task<CartUpdateResult> AddToCartAsync(int productId, int quantity);
        CartUpdateResult UpdateCartItem(int productId, int quantity);
        CartUpdateResult RemoveFromCart(int productId);
        void SaveCartNote(string note);
    }
}
