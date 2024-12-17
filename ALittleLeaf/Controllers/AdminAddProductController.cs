using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminAddProductController : Controller
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IWebHostEnvironment _env;
        public AdminAddProductController(AlittleLeafDecorContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var categories = _context.Categories.Select(c => new CollectionsMenuViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
            })
            .ToList();

            ViewData["categories"] = categories;
            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(AddProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng sản phẩm mới
                var product = new Product
                {
                    ProductName = model.ProductName,
                    IdCategory = model.IdCategory,
                    ProductDescription = model.ProductDescription,
                    QuantityInStock = model.QuantityInStock,
                    ProductPrice = model.ProductPrice,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                // Lưu sản phẩm vào cơ sở dữ liệu
                _context.Products.Add(product);
                _context.SaveChanges();

                // Console log thông báo đã thêm sản phẩm
                Console.WriteLine($"Sản phẩm '{model.ProductName}' đã được thêm vào cơ sở dữ liệu với ID: {product.ProductId}");

                var uploadPath = Path.Combine(_env.WebRootPath, "img", "prdImg"); // Lấy đường dẫn vật lý
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa tồn tại
                }

                bool isFirstImage = true;

                foreach (var image in model.ProductImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName); // Lấy tên file
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Kiểm tra trùng tên và xử lý
                        if (System.IO.File.Exists(filePath))
                        {
                            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                            filePath = Path.Combine(uploadPath, fileName);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        // Lưu đường dẫn vào ProductImage
                        var productImage = new ProductImage
                        {
                            IdProduct = product.ProductId,
                            ImgName = fileName,
                            IsPrimary = isFirstImage, // Nếu là ảnh đầu tiên thì đặt IsPrimary = true
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        _context.ProductImages.Add(productImage);

                        // Đặt isFirstImage = false sau khi lưu ảnh đầu tiên
                        isFirstImage = false;

                        // Console log thông báo đã lưu ảnh
                        Console.WriteLine($"Ảnh '{fileName}' đã được lưu cho sản phẩm '{model.ProductName}'");
                    }
                }

                _context.SaveChanges();

                // Console log khi đã hoàn tất thêm ảnh
                Console.WriteLine($"Tất cả ảnh đã được lưu cho sản phẩm '{model.ProductName}'");

                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ
            Console.WriteLine("Dữ liệu không hợp lệ, không thể thêm sản phẩm.");
            return RedirectToAction("Index");
        }

    }
}
