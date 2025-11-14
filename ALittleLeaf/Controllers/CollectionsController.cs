using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class CollectionsController : SiteBaseController
    {
        private readonly AlittleLeafDecorContext _context;

        public CollectionsController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? CategoryId, int page = 1)
        {
            int pageSize = 12; // Số sản phẩm trên mỗi trang
            var products = _context.Products
                .Include(p => p.ProductImages)
                .AsQueryable();

            string categoryName = "Tất cả sản phẩm";
            string collectionName = "Tất cả sản phẩm";

            if (CategoryId.HasValue)
            {
                var category = _context.Categories
                    .Where(c => c.CategoryId == CategoryId.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefault();

                categoryName = category ?? "";
                collectionName = categoryName ?? "";

                var hasSubCategories = _context.Categories.Any(c => c.CategoryParentId == CategoryId);

                if (hasSubCategories)
                {
                    products = products.Where(p => _context.Categories
                        .Where(c => c.CategoryParentId == CategoryId)
                        .Select(c => c.CategoryId)
                        .Contains(p.IdCategory));
                }
                else
                {
                    products = products.Where(p => p.IdCategory == CategoryId);
                }
            }

            products = products.Where(p => p.IsOnSale); // Chỉ lấy sản phẩm đang mở bán

            int totalItems = products.Count();

            var result = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.QuantityInStock,
                    ProductImg = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName,
                }).ToList();

            var pagination = new Paginate(totalItems, page, pageSize);

            ViewBag.Pagination = pagination;
            ViewBag.CategoryName = categoryName;
            ViewBag.CategoryId = CategoryId;
            ViewBag.CollectionName = collectionName;

            return View(result);
        }
    }
}
