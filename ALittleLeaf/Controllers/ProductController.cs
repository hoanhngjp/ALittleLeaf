using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ALittleLeaf.Controllers
{
    public class ProductController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public ProductController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int productId, int idCategory)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

            ViewData["Cart"] = cart;
            // Sử dụng productId và idCategory trong phương thức
            var product = _context.Products
                .Where(p => p.ProductId == productId)
                .SingleOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            // Lấy tất cả hình ảnh liên quan đến sản phẩm
            var productImages = _context.ProductImages
                .Where(img => img.IdProduct == productId)
                .ToList();

            // Tìm hình ảnh chính (IsPrimary = true)
            var primaryImage = productImages.FirstOrDefault(img => img.IsPrimary)?.ImgName;

            // Lấy thông tin Category
            var category = _context.Categories
                     .Where(c => c.CategoryId == idCategory)
                     .Select(c => c.CategoryName)
                     .FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }

            // Tạo ViewModel và điền dữ liệu
            var viewModel = new ProductDetailViewModel
            {
                ProductId = product.ProductId,
                IdCategory = product.IdCategory,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductDescription = product.ProductDescription,
                QuantityInStock = product.QuantityInStock,
                IsOnSale = product.IsOnSale,
                ProductImages = productImages.Select(img => img.ImgName).ToArray(),
                PrimaryImage = primaryImage,
                CategoryName = category
            };

            return View(viewModel);
        }


    }
}
