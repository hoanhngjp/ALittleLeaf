using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Services.Product;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ALittleLeaf.Controllers
{
    public class ProductController : SiteBaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int productId)
        {
            var viewModel = await _productService.GetProductDetailAsync(productId);

            if (viewModel == null) return NotFound();

            return View(viewModel);
        }
    }
}
