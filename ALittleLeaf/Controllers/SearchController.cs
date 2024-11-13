using Microsoft.AspNetCore.Mvc;
using ALittleLeaf.Models;
using ALittleLeaf.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using ALittleLeaf.Repository;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class SearchController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public SearchController(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string? q, int page = 1)
        {
            if (string.IsNullOrEmpty(q))
            {
                return View(new List<ProductViewModel>());
            }
            else
            {
                int pageSize = 15; // Số sản phẩm trên mỗi trang

                var products = _context.Products
                    .Where(p => p.ProductName.Contains(q))
                    .Include(p => p.ProductImages)
                    .AsQueryable();

                products = products.Where(p => p.IsOnSale); // Chỉ lấy sản phẩm đang mở bán

                int totalItems = products.Count();

                var searchResults = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        IdCategory = p.IdCategory,
                        ProductName = p.ProductName,
                        ProductPrice = p.ProductPrice,
                        ProductImg = p.ProductImages.FirstOrDefault(i => i.IsPrimary).ImgName
                    })
                    .ToList();

                var pagination = new Paginate(totalItems, page, pageSize);

                ViewBag.KeyWords = q;
                ViewBag.Pagination = pagination;
                ViewBag.TotalItems = totalItems;
                ViewBag.Pagination = pagination;
                return View(searchResults);
            } 
        }

        [HttpPost]
        public IActionResult Search(string q)
        {
            // Tìm sản phẩm theo từ khóa
            var products = _context.Products
                .Where(p => p.ProductName.Contains(q))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductImg = p.ProductImages.FirstOrDefault().ImgName // Lấy ảnh đầu tiên làm đại diện
                })
                .ToList();

            ViewBag.SearchKeyword = q; // Truyền từ khóa tìm kiếm vào ViewBag

            return PartialView("_SearchResultsPartial", products);
        }

    }
}
