using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Controllers
{
    public class AdminProductInfoController : Controller
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminProductInfoController(AlittleLeafDecorContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int productId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            // Lấy tất cả danh mục sản phẩm
            var categories = _context.Categories
                .Select(c => new CollectionsMenuViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                })
                .ToList();

            ViewData["categories"] = categories;

            // Lấy thông tin sản phẩm và hình ảnh liên quan
            var productDetail = _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new
                {
                    Product = p,
                    Images = _context.ProductImages
                        .Where(img => img.IdProduct == productId)
                        .ToList()
                })
                .SingleOrDefault();

            if (productDetail == null || productDetail.Product == null)
            {
                return NotFound();
            }

            // Tạo ViewModel và điền dữ liệu
            var viewModel = new EditProductViewModel
            {
                ProductId = productDetail.Product.ProductId,
                ProductName = productDetail.Product.ProductName,
                IdCategory = productDetail.Product.IdCategory,
                ProductPrice = productDetail.Product.ProductPrice,
                ProductDescription = productDetail.Product.ProductDescription,
                QuantityInStock = productDetail.Product.QuantityInStock,
                IsOnSale = productDetail.Product.IsOnSale,
                ExistingProductImages = productDetail.Images.Select(i => new ExistingImageViewModel
                {
                    ImgId = i.ImgId,
                    ImgName = i.ImgName,
                    IsPrimary = i.IsPrimary,
                }).ToList(), // Lưu danh sách ảnh đã tồn tại
                NewProductImages = new List<IFormFile>() // Khởi tạo danh sách ảnh mới rỗng
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(EditProductViewModel model, List<IFormFile>? product_images, List<int>? delete_images)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Lỗi trong {modelState.Key}: {error.ErrorMessage}");
                    }
                }
            }

            // Kiểm tra tính hợp lệ của model
            if (ModelState.IsValid)
            {
                // Kiểm tra xem sản phẩm có tồn tại hay không
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

                if (product == null)
                {
                    Console.WriteLine("Sản phẩm không tồn tại.");
                    ModelState.AddModelError(string.Empty, "Sản phẩm không tồn tại.");
                    return RedirectToAction("Index", new { productId = model.ProductId }); // Trả về lại View với lỗi
                }

                // Cập nhật thông tin sản phẩm
                product.ProductName = model.ProductName;
                product.IdCategory = model.IdCategory;
                product.ProductPrice = model.ProductPrice;
                product.ProductDescription = model.ProductDescription;
                product.QuantityInStock = model.QuantityInStock;
                product.IsOnSale = model.IsOnSale;
                product.UpdatedAt = DateTime.Now;

                // Xử lý ảnh chính
                if (model.IsPrimaryImg.HasValue)
                {
                    var primaryImage = product.ProductImages.FirstOrDefault(img => img.ImgId == model.IsPrimaryImg.Value);
                    if (primaryImage != null)
                    {
                        // Đặt ảnh chính
                        foreach (var img in product.ProductImages)
                        {
                            img.IsPrimary = (img.ImgId == primaryImage.ImgId);
                        }
                    }
                }

                // Kiểm tra nếu có ảnh xóa
                if (delete_images != null && delete_images.Any())
                {
                    var imagesToDelete = product.ProductImages
                        .Where(img => delete_images.Contains(img.ImgId))
                        .ToList();

                    foreach (var image in imagesToDelete)
                    {
                        _context.ProductImages.Remove(image);

                        var imagePath = Path.Combine(_env.WebRootPath, "img", "prdImg", image.ImgName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }

                // Kiểm tra nếu có ảnh mới
                if (product_images != null && product_images.Any())
                {
                    foreach (var image in product_images)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(_env.WebRootPath, "img", "prdImg", fileName);

                        try
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi lưu ảnh: {ex.Message}");
                            ModelState.AddModelError("product_images", "Đã có lỗi khi lưu ảnh.");
                            break;
                        }

                        var newImage = new ProductImage
                        {
                            IdProduct = product.ProductId,
                            ImgName = fileName
                        };

                        _context.ProductImages.Add(newImage);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã có lỗi khi lưu thông tin sản phẩm.");
                }

                return RedirectToAction("Index", new { productId = model.ProductId });
            }

            return RedirectToAction("Index", new { productId = model.ProductId });
        }

    }
}
