using ALittleLeaf.Api.DTOs.Admin;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Admin;
using ALittleLeaf.Api.Services.Cloudinary;
using Microsoft.AspNetCore.Identity;

namespace ALittleLeaf.Api.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository   _adminRepo;
        private readonly ICloudinaryService _cloudinary;

        public AdminService(IAdminRepository adminRepo, ICloudinaryService cloudinary)
        {
            _adminRepo  = adminRepo;
            _cloudinary = cloudinary;
        }

        // ── Mapping helpers ───────────────────────────────────────────────────

        private static AdminProductDto MapProduct(Models.Product p) => new()
        {
            ProductId          = p.ProductId,
            IdCategory         = p.IdCategory,
            CategoryName       = p.IdCategoryNavigation?.CategoryName ?? string.Empty,
            ProductName        = p.ProductName,
            ProductPrice       = p.ProductPrice,
            ProductDescription = p.ProductDescription,
            QuantityInStock    = p.QuantityInStock,
            IsOnSale           = p.IsOnSale,
            CreatedAt          = p.CreatedAt,
            UpdatedAt          = p.UpdatedAt,
            PrimaryImage       = p.ProductImages.FirstOrDefault(i => i.IsPrimary)?.ImgName,
            Images             = p.ProductImages.Select(i => new AdminProductImageDto
            {
                ImgId     = i.ImgId,
                ImgName   = i.ImgName,
                IsPrimary = i.IsPrimary
            })
        };

        private static AdminOrderDto MapOrder(Bill b) => new()
        {
            BillId         = b.BillId,
            UserId         = b.IdUser,
            CustomerName   = b.IdUserNavigation?.UserFullname ?? string.Empty,
            CustomerEmail  = b.IdUserNavigation?.UserEmail    ?? string.Empty,
            DateCreated    = b.DateCreated,
            TotalAmount    = b.TotalAmount,
            PaymentMethod  = b.PaymentMethod,
            PaymentStatus  = b.PaymentStatus,
            IsConfirmed    = b.IsConfirmed,
            ShippingStatus = b.ShippingStatus,
            Note           = b.Note,
            ItemCount      = b.BillDetails?.Count ?? 0
        };

        private static AdminUserDto MapUser(User u) => new()
        {
            UserId       = u.UserId,
            UserEmail    = u.UserEmail,
            UserFullname = u.UserFullname,
            UserSex      = u.UserSex,
            UserBirthday = u.UserBirthday,
            UserIsActive = u.UserIsActive,
            UserRole     = u.UserRole,
            CreatedAt    = u.CreatedAt,
            UpdatedAt    = u.UpdatedAt
        };

        // ── Products ──────────────────────────────────────────────────────────

        public async Task<PaginatedAdminResultDto<AdminProductDto>> GetProductsAsync(
            string? keyword, int? categoryId, bool? isOnSale,
            string? sortBy, bool isDescending,
            int page, int pageSize)
        {
            var (total, items) = await _adminRepo.GetProductsPagedAsync(
                keyword, categoryId, isOnSale, sortBy, isDescending, page, pageSize);

            return new PaginatedAdminResultDto<AdminProductDto>
            {
                TotalItems = total,
                Page       = page,
                PageSize   = pageSize,
                Items      = items.Select(MapProduct)
            };
        }

        public async Task<AdminProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _adminRepo.GetProductByIdAsync(productId);
            return product == null ? null : MapProduct(product);
        }

        public async Task<AdminProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Models.Product
            {
                IdCategory         = dto.IdCategory,
                ProductName        = dto.ProductName,
                ProductPrice       = dto.ProductPrice,
                ProductDescription = dto.ProductDescription,
                QuantityInStock    = dto.QuantityInStock,
                IsOnSale           = dto.IsOnSale,
                CreatedAt          = DateTime.UtcNow,
                UpdatedAt          = DateTime.UtcNow
            };

            var created = await _adminRepo.CreateProductAsync(product);

            // Upload primary image
            if (dto.PrimaryImage != null)
            {
                var url = await _cloudinary.UploadImageAsync(dto.PrimaryImage, "products");
                await _adminRepo.AddProductImageAsync(new ProductImage
                {
                    IdProduct = created.ProductId,
                    ImgName   = url,
                    IsPrimary = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Upload additional images
            if (dto.AdditionalImages != null)
            {
                foreach (var file in dto.AdditionalImages)
                {
                    var url = await _cloudinary.UploadImageAsync(file, "products");
                    await _adminRepo.AddProductImageAsync(new ProductImage
                    {
                        IdProduct = created.ProductId,
                        ImgName   = url,
                        IsPrimary = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await _adminRepo.SaveChangesAsync();

            // Re-fetch to return full DTO with images
            var full = await _adminRepo.GetProductByIdAsync(created.ProductId);
            return MapProduct(full!);
        }

        public async Task<AdminProductDto?> UpdateProductAsync(int productId, UpdateProductDto dto)
        {
            var product = await _adminRepo.GetProductByIdAsync(productId);
            if (product == null) return null;

            product.IdCategory         = dto.IdCategory;
            product.ProductName        = dto.ProductName;
            product.ProductPrice       = dto.ProductPrice;
            product.ProductDescription = dto.ProductDescription;
            product.QuantityInStock    = dto.QuantityInStock;
            product.IsOnSale           = dto.IsOnSale;
            product.UpdatedAt          = DateTime.UtcNow;

            await _adminRepo.UpdateProductAsync(product);

            // Delete specified gallery images from Cloudinary + DB
            if (dto.DeleteImageIds != null)
            {
                foreach (var imgId in dto.DeleteImageIds)
                {
                    var img = await _adminRepo.GetProductImageByIdAsync(imgId);
                    if (img == null || img.IdProduct != productId) continue;

                    var publicId = ExtractPublicId(img.ImgName);
                    if (!string.IsNullOrEmpty(publicId))
                        await _cloudinary.DeleteImageAsync(publicId);

                    await _adminRepo.DeleteProductImageAsync(img);
                }
            }

            // Promote an existing image to primary (only when no new file is being uploaded)
            if (dto.ExistingPrimaryImageId.HasValue && dto.NewPrimaryImage == null)
            {
                var targetId = dto.ExistingPrimaryImageId.Value;
                // Verify the target image belongs to this product
                var targetImg = product.ProductImages.FirstOrDefault(i => i.ImgId == targetId);
                if (targetImg != null && !targetImg.IsPrimary)
                {
                    foreach (var img in product.ProductImages)
                        img.IsPrimary = img.ImgId == targetId;
                }
            }

            // Replace primary image with a newly uploaded file (takes precedence over ExistingPrimaryImageId)
            if (dto.NewPrimaryImage != null)
            {
                var oldPrimary = product.ProductImages.FirstOrDefault(i => i.IsPrimary);
                if (oldPrimary != null)
                {
                    var publicId = ExtractPublicId(oldPrimary.ImgName);
                    if (!string.IsNullOrEmpty(publicId))
                        await _cloudinary.DeleteImageAsync(publicId);

                    await _adminRepo.DeleteProductImageAsync(oldPrimary);
                }

                var url = await _cloudinary.UploadImageAsync(dto.NewPrimaryImage, "products");
                await _adminRepo.AddProductImageAsync(new ProductImage
                {
                    IdProduct = productId,
                    ImgName   = url,
                    IsPrimary = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Append additional images
            if (dto.AdditionalImages != null)
            {
                foreach (var file in dto.AdditionalImages)
                {
                    var url = await _cloudinary.UploadImageAsync(file, "products");
                    await _adminRepo.AddProductImageAsync(new ProductImage
                    {
                        IdProduct = productId,
                        ImgName   = url,
                        IsPrimary = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await _adminRepo.SaveChangesAsync();

            var updated = await _adminRepo.GetProductByIdAsync(productId);
            return MapProduct(updated!);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _adminRepo.GetProductByIdAsync(productId);
            if (product == null) return false;

            // Delete all images from Cloudinary
            foreach (var img in product.ProductImages)
            {
                var publicId = ExtractPublicId(img.ImgName);
                if (!string.IsNullOrEmpty(publicId))
                    await _cloudinary.DeleteImageAsync(publicId);
            }

            await _adminRepo.DeleteProductAsync(product);
            await _adminRepo.SaveChangesAsync();
            return true;
        }

        // ── Orders ────────────────────────────────────────────────────────────

        public async Task<PaginatedAdminResultDto<AdminOrderDto>> GetOrdersAsync(
            string? keyword, string? shippingStatus, string? paymentStatus,
            DateOnly? startDate, DateOnly? endDate,
            string? sortBy, bool isDescending,
            int page, int pageSize)
        {
            var (total, items) = await _adminRepo.GetOrdersPagedAsync(
                keyword, shippingStatus, paymentStatus,
                startDate, endDate,
                sortBy, isDescending,
                page, pageSize);

            return new PaginatedAdminResultDto<AdminOrderDto>
            {
                TotalItems = total,
                Page       = page,
                PageSize   = pageSize,
                Items      = items.Select(MapOrder)
            };
        }

        public async Task<AdminOrderDetailDto?> GetOrderByIdAsync(int billId)
        {
            var bill = await _adminRepo.GetOrderByIdAsync(billId);
            if (bill == null) return null;

            var adrs = bill.IdAdrsNavigation;
            var shippingAddress = adrs == null
                ? string.Empty
                : $"{adrs.AdrsFullname} | {adrs.AdrsPhone} | {adrs.AdrsAddress}";

            return new AdminOrderDetailDto
            {
                BillId          = bill.BillId,
                UserId          = bill.IdUser,
                CustomerName    = bill.IdUserNavigation?.UserFullname ?? string.Empty,
                CustomerEmail   = bill.IdUserNavigation?.UserEmail    ?? string.Empty,
                DateCreated     = bill.DateCreated,
                TotalAmount     = bill.TotalAmount,
                PaymentMethod   = bill.PaymentMethod,
                PaymentStatus   = bill.PaymentStatus,
                IsConfirmed     = bill.IsConfirmed,
                ShippingStatus  = bill.ShippingStatus,
                Note            = bill.Note,
                ItemCount       = bill.BillDetails?.Count ?? 0,
                ShippingAddress = shippingAddress,
                Items           = (bill.BillDetails ?? []).Select(bd => new AdminOrderLineItemDto
                {
                    BillDetailId = bd.BillDetailId,
                    ProductId    = bd.IdProduct,
                    ProductName  = bd.IdProductNavigation?.ProductName ?? string.Empty,
                    ProductImg   = bd.IdProductNavigation?.ProductImages
                                     .FirstOrDefault(i => i.IsPrimary)?.ImgName,
                    Quantity     = bd.Quantity,
                    UnitPrice    = bd.UnitPrice,
                    TotalPrice   = bd.TotalPrice
                })
            };
        }

        public async Task<AdminOrderDto?> UpdateOrderStatusAsync(int billId, UpdateOrderStatusDto dto)
        {
            var bill = await _adminRepo.GetOrderByIdAsync(billId);
            if (bill == null) return null;

            if (dto.ShippingStatus != null) bill.ShippingStatus = dto.ShippingStatus;
            if (dto.PaymentStatus  != null) bill.PaymentStatus  = dto.PaymentStatus;
            if (dto.IsConfirmed    != null) bill.IsConfirmed     = dto.IsConfirmed.Value;
            bill.UpdatedAt = DateTime.UtcNow;

            await _adminRepo.UpdateOrderAsync(bill);
            await _adminRepo.SaveChangesAsync();
            return MapOrder(bill);
        }

        // ── Users ─────────────────────────────────────────────────────────────

        public async Task<PaginatedAdminResultDto<AdminUserDto>> GetUsersAsync(
            string? keyword, bool? isActive, string? userRole, bool? userSex,
            string? sortBy, bool isDescending,
            int page, int pageSize)
        {
            var (total, items) = await _adminRepo.GetUsersPagedAsync(
                keyword, isActive, userRole, userSex, sortBy, isDescending, page, pageSize);

            return new PaginatedAdminResultDto<AdminUserDto>
            {
                TotalItems = total,
                Page       = page,
                PageSize   = pageSize,
                Items      = items.Select(MapUser)
            };
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(long userId)
        {
            var user = await _adminRepo.GetUserByIdAsync(userId);
            return user == null ? null : MapUser(user);
        }

        public async Task<(AdminUserDto? User, string? Error)> CreateUserAsync(AdminCreateUserDto dto)
        {
            if (await _adminRepo.UserEmailExistsAsync(dto.UserEmail))
                return (null, "Email đã được sử dụng.");

            var hasher = new PasswordHasher<string>();
            var user = new User
            {
                UserEmail    = dto.UserEmail,
                UserPassword = hasher.HashPassword(null!, dto.Password),
                UserFullname = dto.UserFullname,
                UserSex      = dto.UserSex,
                UserBirthday = dto.UserBirthday ?? DateOnly.FromDateTime(DateTime.UtcNow),
                UserIsActive = dto.UserIsActive,
                UserRole     = dto.UserRole,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow,
            };

            var created = await _adminRepo.CreateUserAsync(user);
            return (MapUser(created), null);
        }

        public async Task<AdminUserDto?> UpdateUserAsync(long userId, AdminUpdateUserDto dto)
        {
            var user = await _adminRepo.GetUserByIdAsync(userId);
            if (user == null) return null;

            if (dto.UserFullname != null) user.UserFullname = dto.UserFullname;
            if (dto.UserSex      != null) user.UserSex      = dto.UserSex.Value;
            if (dto.UserBirthday != null) user.UserBirthday = dto.UserBirthday.Value;
            if (dto.UserIsActive != null) user.UserIsActive = dto.UserIsActive.Value;
            if (dto.UserRole     != null) user.UserRole     = dto.UserRole;
            user.UpdatedAt = DateTime.UtcNow;

            await _adminRepo.UpdateUserAsync(user);
            await _adminRepo.SaveChangesAsync();
            return MapUser(user);
        }

        // ── Dashboard ─────────────────────────────────────────────────────────

        public async Task<DashboardDto> GetDashboardAsync(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var revenue           = await _adminRepo.GetTotalRevenueAsync(startDate, endDate);
            var totalOrders       = await _adminRepo.GetTotalOrderCountAsync(startDate, endDate);
            var pendingOrders     = await _adminRepo.GetPendingOrderCountAsync(startDate, endDate);
            var totalUsers        = await _adminRepo.GetTotalUserCountAsync();
            var totalProducts     = await _adminRepo.GetTotalProductCountAsync();
            var lowStock          = await _adminRepo.GetLowStockProductsAsync();
            var topSelling        = await _adminRepo.GetTopSellingProductsAsync();
            var revenueByMonth    = await _adminRepo.GetRevenueByMonthAsync(startDate, endDate);
            var revenueByCategory = await _adminRepo.GetRevenueByCategoryAsync(startDate, endDate);

            return new DashboardDto
            {
                TotalRevenue  = revenue,
                TotalOrders   = totalOrders,
                PendingOrders = pendingOrders,
                TotalUsers    = totalUsers,
                TotalProducts = totalProducts,
                LowStockProducts = lowStock.Select(p => new LowStockProductDto
                {
                    ProductId       = p.ProductId,
                    ProductName     = p.ProductName,
                    QuantityInStock = p.QuantityInStock,
                    PrimaryImage    = p.ProductImages.FirstOrDefault(i => i.IsPrimary)?.ImgName
                }),
                TopSellingProducts = topSelling.Select(t => new TopSellingProductDto
                {
                    ProductId    = t.ProductId,
                    ProductName  = t.ProductName,
                    TotalSold    = t.TotalSold,
                    TotalRevenue = t.TotalRevenue,
                    PrimaryImage = t.PrimaryImage
                }),
                RevenueByMonth = revenueByMonth.Select(r => new RevenueByMonthDto
                {
                    Year    = r.Year,
                    Month   = r.Month,
                    Revenue = r.Revenue
                }),
                RevenueByCategory = revenueByCategory.Select(c => new RevenueByCategoryDto
                {
                    CategoryName = c.CategoryName,
                    Revenue      = c.Revenue
                })
            };
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Extracts the Cloudinary public ID from a secure URL.
        /// Example: https://res.cloudinary.com/demo/image/upload/v123/products/abc.jpg → products/abc
        /// </summary>
        private static string? ExtractPublicId(string url)
        {
            try
            {
                var uri      = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                // Find the "upload" segment; everything after is version (optional) + public_id + extension
                var uploadIdx = Array.IndexOf(segments, "upload");
                if (uploadIdx < 0 || uploadIdx + 1 >= segments.Length) return null;

                // Skip version segment (starts with 'v' + digits)
                var start = uploadIdx + 1;
                if (start < segments.Length && segments[start].StartsWith('v') &&
                    int.TryParse(segments[start][1..], out _))
                    start++;

                var publicIdWithExt = string.Join("/", segments[start..]);
                var dot             = publicIdWithExt.LastIndexOf('.');
                return dot > 0 ? publicIdWithExt[..dot] : publicIdWithExt;
            }
            catch
            {
                return null;
            }
        }
    }
}
