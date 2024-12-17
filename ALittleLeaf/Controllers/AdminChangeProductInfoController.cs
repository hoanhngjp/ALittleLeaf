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
        private readonly IWebHostEnvironment _env;

        public AdminChangeProductInfoController(AlittleLeafDecorContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
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
                    CreatedDate = p.CreatedAt,
                    UpdatedDate = p.UpdatedAt,
                }).ToList();

            int totalItems = products.Count();

            var pagination = new Paginate(totalItems, page, pageSize);

            ViewBag.Pagination = pagination;
            

            return View(result);
        }
        [HttpGet]
        public IActionResult DeleteProduct(int productId)
        {
            // Tìm sản phẩm trong bảng Products
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                // Nếu không tìm thấy sản phẩm, quay lại trang Index
                return RedirectToAction("Index");
            }

            // Thực hiện xóa mềm: Cập nhật trường IsOnSale thành false
            product.IsOnSale = false;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "AdminChangeProductInfo");
        }
        [HttpGet]
        public IActionResult SearchProduct(string searchType, string searchKey, int page = 1, int pageSize = 10)
        {
            var productQuery = _context.Products
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Áp dụng logic tìm kiếm
            if (!string.IsNullOrEmpty(searchKey))
            {
                switch (searchType)
                {
                    case "findByProductID":
                        productQuery = productQuery.Where(u => u.ProductId.ToString().Contains(searchKey));
                        break;
                    case "findByProductName":
                        productQuery = productQuery.Where(u => u.ProductName.Contains(searchKey));
                        break;
                }
            }

            // Tổng số bản ghi
            int totalItems = productQuery.Count();

            // Lấy danh sách các user thuộc trang hiện tại
            var products = productQuery
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
                    CreatedDate = p.CreatedAt,
                    UpdatedDate = p.UpdatedAt,
                }).ToList();

            // Nếu không có kết quả, tạo ViewBag chứa thông báo
            if (!products.Any())
            {
                ViewBag.Message = "Không tìm thấy sản phẩm nào phù hợp với từ khóa tìm kiếm.";
            }

            // Tạo ViewModel để truyền dữ liệu
            var model = new ProductSearchViewModel
            {
                Products = products,
                Pagination = new Paginate(totalItems, page, pageSize)
            };
            return PartialView("_AdminSearchProductResult", model);
        }
    }
}
