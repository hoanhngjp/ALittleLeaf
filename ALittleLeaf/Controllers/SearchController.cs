using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Product;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ALittleLeaf.Controllers
{
    public class SearchController : SiteBaseController
    {
        private readonly IProductService _productService;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            if (string.IsNullOrEmpty(q))
            {
                // Trả về list rỗng nếu không có từ khóa
                return View(new List<ProductViewModel>());
            }

            // Gọi Service
            var result = await _productService.GetPaginatedProductsAsync(null, q, page, 15);

            if (result.Pagination.TotalItems == 0)
            {
                ViewBag.NoResults = true;
            }

            // Gán ViewBag cho View cũ hoạt động bình thường
            ViewBag.KeyWords = q;
            ViewBag.TotalItems = result.Pagination.TotalItems;
            ViewBag.Pagination = result.Pagination;

            // Trả về List<ProductViewModel> như cũ
            return View(result.Products);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string q)
        {
            var products = await _productService.GetLiveSearchResultsAsync(q);

            ViewBag.SearchKeyword = q;
            return PartialView("_SearchResultsPartial", products);
        }
    }
}
