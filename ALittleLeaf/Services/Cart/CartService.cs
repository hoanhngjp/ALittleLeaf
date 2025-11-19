using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(AlittleLeafDecorContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        // --- Các hàm private helper ---
        private List<CartItemViewModel> GetCart()
        {
            return Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            Session.SetObjectAsJson("Cart", cart);
        }

        public List<CartItemViewModel> GetCartItems()
        {
            return GetCart();
        }

        public int GetCartTotal()
        {
            var cart = GetCart();
            return cart.Sum(c => c.Quantity * c.ProductPrice);
        }

        public int GetCartItemCount()
        {
            var cart = GetCart();
            return cart.Sum(c => c.Quantity);
        }

        public async Task<CartUpdateResult> AddToCartAsync(int productId, int quantity)
        {
            // Logic từ [HttpPost] AddToCart
            var product = await _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new CartItemViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    Quantity = quantity,
                    ProductImg = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return new CartUpdateResult { Success = false, Message = "Sản phẩm không tồn tại." };
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(product);
            }

            SaveCart(cart);

            return new CartUpdateResult
            {
                Success = true,
                TotalPrice = GetCartTotal(),
                TotalItems = GetCartItemCount()
            };
        }

        public CartUpdateResult UpdateCartItem(int productId, int quantity)
        {
            // Logic từ [HttpPost] UpdateCartItem
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                return new CartUpdateResult { Success = false, Message = "Giỏ hàng rỗng" };
            }

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null)
            {
                return new CartUpdateResult { Success = false, Message = "Sản phẩm không tìm thấy trong giỏ hàng" };
            }

            item.Quantity = quantity;
            SaveCart(cart);

            return new CartUpdateResult
            {
                Success = true,
                LineItemTotal = item.Quantity * item.ProductPrice,
                TotalPrice = GetCartTotal(),
                TotalItems = GetCartItemCount()
            };
        }

        public CartUpdateResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }

            return new CartUpdateResult
            {
                Success = true,
                TotalPrice = GetCartTotal(),
                TotalItems = GetCartItemCount()
            };
        }

        public void SaveCartNote(string note)
        {
            Session.SetString("BillNote", note ?? string.Empty);
        }
    }
}
