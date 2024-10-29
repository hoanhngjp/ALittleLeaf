using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public CollectionsController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? CategoryId)
        {
            var products = _context.Products
                .Include(p => p.ProductImages)
                .AsQueryable();

            string categoryName = "Tất cả sản phẩm"; // Khai báo biến để lưu tên danh mục
            string collectionName = "Tất cả sản phẩm"; // Khai báo biến để lưu tên Collection

            // Kiểm tra xem có CategoryId không
            if (CategoryId.HasValue)
            {
                // Lấy tên danh mục từ cơ sở dữ 
                var category = _context.Categories
            .Where(c => c.CategoryId == CategoryId.Value)
            .Select(c => c.CategoryName)
            .FirstOrDefault(); // Lấy trực tiếp tên danh mục mà không tạo cột phụ

                categoryName = category ?? ""; // Nếu không tìm thấy, gán là Tất cả sản phẩm
                collectionName = categoryName ?? ""; // Nếu không tìm thấy, gán là Tất cả sản phẩm

                // Kiểm tra xem CategoryId có phải là danh mục cha không
                var hasSubCategories = _context.Categories.Any(c => c.CategoryParentId == CategoryId);

                if (hasSubCategories)
                {
                    // Nếu là danh mục cha, lấy tất cả sản phẩm của danh mục con
                    products = products.Where(p => _context.Categories
                        .Where(c => c.CategoryParentId == CategoryId)
                        .Select(c => c.CategoryId)
                        .Contains(p.IdCategory));
                }
                else
                {
                    // Nếu là danh mục con, chỉ lấy sản phẩm thuộc danh mục đó
                    products = products.Where(p => p.IdCategory == CategoryId);
                }
            }

            // Lọc các sản phẩm đang giảm giá
            products = products.Where(p => p.IsOnSale);

            var result = products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                IdCategory = p.IdCategory,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                ProductImg = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName,
            }).ToList();

            ViewBag.CategoryName = categoryName; // Truyền tên danh mục vào ViewBag
            ViewBag.CollectionName = collectionName;

            return View(result);
        }



    }
}
