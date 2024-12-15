using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace ALittleLeaf.Controllers
{
    public class AdminChangeProductInfoController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminChangeProductInfoController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10; 
            var products = _context.Products
                .Include(p => p.ProductImages)
                .AsQueryable();

            var result = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDetailViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductDescription = p.ProductDescription,
                    QuantityInStock = p.QuantityInStock,
                    IsOnSale = p.IsOnSale,
                    PrimaryImage = p.ProductImages.FirstOrDefault(img => img.IsPrimary).ImgName,
                }).ToList();

            int totalItems = products.Count();

            var pagination = new Paginate(totalItems, page, pageSize);

            ViewBag.Pagination = pagination;
            

            return View(result);
        }
    }
}
