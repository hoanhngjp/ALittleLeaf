using ALittleLeaf.Areas.Admin.ViewModels;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IWebHostEnvironment _env;

        // Gộp các dependency từ 4 controller
        public ProductsController(AlittleLeafDecorContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //--- TỪ AdminChangeProductInfoController ---
        // GET: /Admin/Products
        // GET: /Admin/Products/Index
        [HttpGet]
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
                    CreatedDate = p.CreatedAt,
                    UpdatedDate = p.UpdatedAt,
                }).ToList();

            int totalItems = products.Count();
            var pagination = new Paginate(totalItems, page, pageSize);
            ViewBag.Pagination = pagination;

            // Sẽ dùng View tại: Areas/Admin/Views/Products/Index.cshtml
            return View(result);
        }

        //--- TỪ AdminChangeProductInfoController ---
        // GET: /Admin/Products/SearchProduct
        [HttpGet]
        public IActionResult SearchProduct(string searchType, string searchKey, int page = 1, int pageSize = 10)
        {
            var productQuery = _context.Products
                .Include(p => p.ProductImages)
                .AsQueryable();

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

            int totalItems = productQuery.Count();
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

            if (!products.Any())
            {
                ViewBag.Message = "Không tìm thấy sản phẩm nào phù hợp với từ khóa tìm kiếm.";
            }

            var model = new ProductSearchViewModel
            {
                Products = products,
                Pagination = new Paginate(totalItems, page, pageSize)
            };
            return PartialView("_AdminSearchProductResult", model);
        }


        //--- TỪ AdminAddProductController ---
        // GET: /Admin/Products/Create
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.Select(c => new CollectionsMenuViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
            })
            .ToList();

            ViewData["categories"] = categories;

            // Sẽ dùng View tại: Areas/Admin/Views/Products/Create.cshtml
            return View();
        }

        //--- TỪ AdminAddProductController ---
        // POST: /Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm bảo mật
        public IActionResult Create(AddProductViewModel model) // Đổi tên Action từ "AddProduct"
        {
            if (ModelState.IsValid)
            {
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

                _context.Products.Add(product);
                _context.SaveChanges();

                var uploadPath = Path.Combine(_env.WebRootPath, "img", "prdImg");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                bool isFirstImage = true;

                foreach (var image in model.ProductImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                            filePath = Path.Combine(uploadPath, fileName);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        var productImage = new ProductImage
                        {
                            IdProduct = product.ProductId,
                            ImgName = fileName,
                            IsPrimary = isFirstImage,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        _context.ProductImages.Add(productImage);
                        isFirstImage = false;
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng về trang Index mới
            }

            // Nếu model state không hợp lệ, quay lại form Create với dữ liệu đã nhập
            // (Tải lại categories cho View)
            var categories = _context.Categories.Select(c => new CollectionsMenuViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
            }).ToList();
            ViewData["categories"] = categories;
            return View(model);
        }

        //--- TỪ AdminProductInfoController ---
        // GET: /Admin/Products/Edit/
        [HttpGet]
        public IActionResult Edit(int id) // Đổi tên Action từ "Index"
        {
            var categories = _context.Categories
                .Select(c => new CollectionsMenuViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                })
                .ToList();
            ViewData["categories"] = categories;

            var productDetail = _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    Product = p,
                    Images = _context.ProductImages
                                .Where(img => img.IdProduct == id)
                                .ToList()
                })
                .SingleOrDefault();

            if (productDetail == null || productDetail.Product == null)
            {
                return NotFound();
            }

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
                }).ToList(),
                NewProductImages = new List<IFormFile>()
            };

            return View(viewModel);
        }

        // POST: /Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model, List<IFormFile>? product_images, List<int>? delete_images) // Đổi tên Action từ "EditProduct"
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

                if (product == null)
                {
                    return NotFound();
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
                    foreach (var img in product.ProductImages)
                    {
                        img.IsPrimary = (img.ImgId == model.IsPrimaryImg.Value);
                    }
                }

                // Xử lý xóa ảnh
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

                // Xử lý thêm ảnh mới
                if (product_images != null && product_images.Any())
                {
                    foreach (var image in product_images)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(_env.WebRootPath, "img", "prdImg", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var newImage = new ProductImage
                        {
                            IdProduct = product.ProductId,
                            ImgName = fileName,
                            IsPrimary = !product.ProductImages.Any(pi => pi.IsPrimary) && product.ProductImages.Count == 0
                        };
                        _context.ProductImages.Add(newImage);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Edit", new { id = model.ProductId });
            }

            return RedirectToAction("Edit", new { id = model.ProductId });
        }

        [HttpPost] // <-- ĐỔI SANG POST ĐỂ BẢO MẬT
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Thực hiện xóa mềm
            product.IsOnSale = false;
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Admin/Products/Statistics
        public async Task<IActionResult> Statistics()
        {
            // 1. Dữ liệu 3 Box (dùng async)
            int totalProducts = await _context.Products.CountAsync();
            int totalSoldProducts = await _context.BillDetails.SumAsync(bd => bd.Quantity);
            int totalStock = await _context.Products.SumAsync(p => p.QuantityInStock);

            // 2. Dữ liệu Biểu đồ 1: Top 10 Bán Chạy (giữ nguyên logic)
            var topSellingProducts = await _context.BillDetails
                .GroupBy(bd => new { bd.IdProduct, bd.IdProductNavigation.ProductName })
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key.IdProduct,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(bd => bd.Quantity),
                    Stock = _context.Products.FirstOrDefault(p => p.ProductId == g.Key.IdProduct).QuantityInStock
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(10)
                .ToListAsync();

            // 3. THÊM MỚI: Dữ liệu Biểu đồ 2: Top 10 Tồn Kho Thấp
            var lowStockProducts = await _context.Products
                .Where(p => p.IsOnSale == true) // Chỉ lấy sản phẩm đang bán
                .OrderBy(p => p.QuantityInStock)
                .Take(10)
                .Select(p => new LowStockProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    QuantityInStock = p.QuantityInStock
                })
                .ToListAsync();

            // 4. THÊM MỚI: Dữ liệu Biểu đồ 3: Doanh thu theo Danh mục
            var categorySalesQuery = await _context.BillDetails
                .Include(bd => bd.IdProductNavigation)
                    .ThenInclude(p => p.IdCategoryNavigation)
                .GroupBy(bd => bd.IdProductNavigation.IdCategoryNavigation.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    QuantitySold = g.Sum(bd => bd.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToListAsync();

            // Xử lý dữ liệu cho biểu đồ tròn (Doughnut chart)
            var categoryLabels = new List<string>();
            var categoryData = new List<int>();
            var categoryColors = new List<string>();
            var baseColors = new List<string> {
            "#007bff", "#28a745", "#ffc107", "#dc3545", "#17a2b8",
            "#6c757d", "#fd7e14", "#6610f2", "#e83e8c", "#20c997"
            };

            for (int i = 0; i < categorySalesQuery.Count; i++)
            {
                categoryLabels.Add(categorySalesQuery[i].CategoryName);
                categoryData.Add(categorySalesQuery[i].QuantitySold);
                categoryColors.Add(baseColors[i % baseColors.Count]); // Lặp lại màu
            }

            // 5. Gửi tất cả ra View
            var model = new ProductStatisticsViewModel
            {
                TotalProducts = totalProducts,
                TotalSoldProducts = totalSoldProducts,
                TotalStock = totalStock,
                TopSellingProducts = topSellingProducts,
                LowStockProducts = lowStockProducts, // <-- Gán dữ liệu mới
                CategorySalesLabels = categoryLabels, // <-- Gán dữ liệu mới
                CategorySalesData = categoryData, // <-- Gán dữ liệu mới
                CategorySalesColors = categoryColors // <-- Gán dữ liệu mới
            };

            return View(model);
        }
    }
}
