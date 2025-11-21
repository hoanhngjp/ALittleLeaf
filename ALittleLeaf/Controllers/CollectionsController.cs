using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Product;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class CollectionsController : SiteBaseController
    {
        private readonly IProductService _productService;

        public CollectionsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? CategoryId, int? minPrice, int? maxPrice, int page = 1)
        {
            // Gọi Service
            var result = await _productService.GetPaginatedProductsAsync(CategoryId, null, minPrice, maxPrice, page, 12);

            // Gán ViewBag cho View cũ hoạt động bình thường
            ViewBag.Pagination = result.Pagination;
            ViewBag.CategoryName = result.PageTitle;
            ViewBag.CollectionName = result.PageTitle;
            ViewBag.CategoryId = result.CategoryId;

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Trả về List<ProductViewModel> như cũ
            return View(result.Products);
        }
    }
}
