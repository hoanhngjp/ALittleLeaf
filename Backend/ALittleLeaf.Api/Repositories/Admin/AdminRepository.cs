using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        // ── Products ──────────────────────────────────────────────────────────

        public async Task<(int Total, IEnumerable<Models.Product> Items)> GetProductsPagedAsync(
            string? keyword, int? categoryId, bool? isOnSale,
            string? sortBy, bool isDescending,
            int page, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.IdCategoryNavigation)
                .AsQueryable();

            // ── Filtering ────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    (p.IdCategoryNavigation != null &&
                     p.IdCategoryNavigation.CategoryName.Contains(keyword)));

            if (categoryId.HasValue)
                query = query.Where(p => p.IdCategory == categoryId.Value);

            if (isOnSale.HasValue)
                query = query.Where(p => p.IsOnSale == isOnSale.Value);

            var total = await query.CountAsync();

            // ── Sorting (before Skip/Take) ───────────────────────────────────
            IOrderedQueryable<Models.Product> sorted = (sortBy?.ToLowerInvariant()) switch
            {
                "productname" or "name" => isDescending
                    ? query.OrderByDescending(p => p.ProductName)
                    : query.OrderBy(p => p.ProductName),

                "productprice" or "price" => isDescending
                    ? query.OrderByDescending(p => p.ProductPrice)
                    : query.OrderBy(p => p.ProductPrice),

                "quantityinstock" or "stock" => isDescending
                    ? query.OrderByDescending(p => p.QuantityInStock)
                    : query.OrderBy(p => p.QuantityInStock),

                "updatedat" => isDescending
                    ? query.OrderByDescending(p => p.UpdatedAt)
                    : query.OrderBy(p => p.UpdatedAt),

                // default: newest first
                _ => isDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
            };

            var items = await sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, items);
        }

        public Task<Models.Product?> GetProductByIdAsync(int productId)
            => _context.Products
                       .Include(p => p.ProductImages)
                       .Include(p => p.IdCategoryNavigation)
                       .FirstOrDefaultAsync(p => p.ProductId == productId);

        public async Task<Models.Product> CreateProductAsync(Models.Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public Task UpdateProductAsync(Models.Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(Models.Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }

        public Task<ProductImage?> GetProductImageByIdAsync(int imgId)
            => _context.ProductImages.FirstOrDefaultAsync(i => i.ImgId == imgId);

        public Task AddProductImageAsync(ProductImage image)
        {
            _context.ProductImages.Add(image);
            return Task.CompletedTask;
        }

        public Task DeleteProductImageAsync(ProductImage image)
        {
            _context.ProductImages.Remove(image);
            return Task.CompletedTask;
        }

        // ── Orders ────────────────────────────────────────────────────────────

        public async Task<(int Total, IEnumerable<Bill> Items)> GetOrdersPagedAsync(
            string? keyword, string? shippingStatus, string? paymentStatus,
            DateOnly? startDate, DateOnly? endDate,
            string? sortBy, bool isDescending,
            int page, int pageSize)
        {
            var query = _context.Bills
                .Include(b => b.IdUserNavigation)
                .Include(b => b.BillDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(b =>
                    b.BillId.ToString().Contains(keyword) ||
                    (b.IdUserNavigation != null && b.IdUserNavigation.UserFullname.Contains(keyword)));

            if (!string.IsNullOrWhiteSpace(shippingStatus))
                query = query.Where(b => b.ShippingStatus == shippingStatus);

            if (!string.IsNullOrWhiteSpace(paymentStatus))
                query = query.Where(b => b.PaymentStatus == paymentStatus);

            if (startDate.HasValue)
                query = query.Where(b => b.DateCreated >= startDate.Value);

            // Include the full end date day (DateOnly has no time, so <= is already inclusive)
            if (endDate.HasValue)
                query = query.Where(b => b.DateCreated <= endDate.Value);

            var total = await query.CountAsync();

            IOrderedQueryable<Bill> sorted = (sortBy?.ToLowerInvariant()) switch
            {
                "billid" or "id" => isDescending
                    ? query.OrderByDescending(b => b.BillId)
                    : query.OrderBy(b => b.BillId),

                "totalamount" or "amount" => isDescending
                    ? query.OrderByDescending(b => b.TotalAmount)
                    : query.OrderBy(b => b.TotalAmount),

                "customername" or "name" => isDescending
                    ? query.OrderByDescending(b => b.IdUserNavigation!.UserFullname)
                    : query.OrderBy(b => b.IdUserNavigation!.UserFullname),

                _ => isDescending
                    ? query.OrderByDescending(b => b.DateCreated)
                    : query.OrderBy(b => b.DateCreated),
            };

            var items = await sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, items);
        }

        public Task<Bill?> GetOrderByIdAsync(int billId)
            => _context.Bills
                       .Include(b => b.IdUserNavigation)
                       .Include(b => b.IdAdrsNavigation)
                       .Include(b => b.BillDetails)
                           .ThenInclude(bd => bd.IdProductNavigation)
                               .ThenInclude(p => p.ProductImages)
                       .FirstOrDefaultAsync(b => b.BillId == billId);

        public Task UpdateOrderAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            return Task.CompletedTask;
        }

        // ── Users ─────────────────────────────────────────────────────────────

        public async Task<(int Total, IEnumerable<User> Items)> GetUsersPagedAsync(
            string? keyword, bool? isActive, string? userRole, bool? userSex,
            string? sortBy, bool isDescending,
            int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(u =>
                    u.UserFullname.Contains(keyword) ||
                    u.UserEmail.Contains(keyword)    ||
                    u.UserId.ToString().Contains(keyword));

            if (isActive.HasValue)
                query = query.Where(u => u.UserIsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(userRole))
                query = query.Where(u => u.UserRole == userRole);

            if (userSex.HasValue)
                query = query.Where(u => u.UserSex == userSex.Value);

            IOrderedQueryable<User> sorted = (sortBy?.ToLowerInvariant()) switch
            {
                "userid" or "id"         => isDescending ? query.OrderByDescending(u => u.UserId)       : query.OrderBy(u => u.UserId),
                "userfullname" or "name" => isDescending ? query.OrderByDescending(u => u.UserFullname) : query.OrderBy(u => u.UserFullname),
                "useremail" or "email"   => isDescending ? query.OrderByDescending(u => u.UserEmail)    : query.OrderBy(u => u.UserEmail),
                "userrole" or "role"     => isDescending ? query.OrderByDescending(u => u.UserRole)     : query.OrderBy(u => u.UserRole),
                "updatedat"              => isDescending ? query.OrderByDescending(u => u.UpdatedAt)    : query.OrderBy(u => u.UpdatedAt),
                _                        => isDescending ? query.OrderByDescending(u => u.CreatedAt)    : query.OrderBy(u => u.CreatedAt),
            };

            var total = await sorted.CountAsync();
            var items = await sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (total, items);
        }

        public Task<User?> GetUserByIdAsync(long userId)
            => _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        public Task<bool> UserEmailExistsAsync(string email)
            => _context.Users.AnyAsync(u => u.UserEmail == email);

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        // ── Dashboard ─────────────────────────────────────────────────────────

        public Task<long> GetTotalRevenueAsync(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var q = _context.Bills.Where(b => b.PaymentStatus == "paid");
            if (startDate.HasValue) q = q.Where(b => b.DateCreated >= startDate.Value);
            if (endDate.HasValue)   q = q.Where(b => b.DateCreated <= endDate.Value);
            return q.SumAsync(b => (long)b.TotalAmount);
        }

        public Task<int> GetTotalOrderCountAsync(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var q = _context.Bills.AsQueryable();
            if (startDate.HasValue) q = q.Where(b => b.DateCreated >= startDate.Value);
            if (endDate.HasValue)   q = q.Where(b => b.DateCreated <= endDate.Value);
            return q.CountAsync();
        }

        public Task<int> GetPendingOrderCountAsync(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var q = _context.Bills.Where(b => b.ShippingStatus == "pending");
            if (startDate.HasValue) q = q.Where(b => b.DateCreated >= startDate.Value);
            if (endDate.HasValue)   q = q.Where(b => b.DateCreated <= endDate.Value);
            return q.CountAsync();
        }

        public Task<int> GetTotalUserCountAsync()
            => _context.Users.CountAsync();

        public Task<int> GetTotalProductCountAsync()
            => _context.Products.CountAsync();

        public Task<IEnumerable<Models.Product>> GetLowStockProductsAsync(int threshold = 5, int take = 10)
            => _context.Products
                       .Include(p => p.ProductImages)
                       .Where(p => p.QuantityInStock <= threshold)
                       .OrderBy(p => p.QuantityInStock)
                       .Take(take)
                       .ToListAsync()
                       .ContinueWith(t => (IEnumerable<Models.Product>)t.Result);

        public async Task<IEnumerable<(int ProductId, string ProductName, int TotalSold, long TotalRevenue, string? PrimaryImage)>>
            GetTopSellingProductsAsync(int take = 5)
        {
            var rows = await _context.BillDetails
                .Include(bd => bd.IdProductNavigation)
                    .ThenInclude(p => p.ProductImages)
                .GroupBy(bd => bd.IdProduct)
                .Select(g => new
                {
                    ProductId    = g.Key,
                    ProductName  = g.First().IdProductNavigation.ProductName,
                    TotalSold    = g.Sum(bd => bd.Quantity),
                    TotalRevenue = (long)g.Sum(bd => bd.TotalPrice),
                    PrimaryImage = g.First().IdProductNavigation.ProductImages
                                    .Where(i => i.IsPrimary)
                                    .Select(i => i.ImgName)
                                    .FirstOrDefault()
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(take)
                .ToListAsync();

            return rows.Select(r => (r.ProductId, r.ProductName, r.TotalSold, r.TotalRevenue, r.PrimaryImage));
        }

        public async Task<IEnumerable<(int Year, int Month, long Revenue)>> GetRevenueByMonthAsync(
            DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var defaultStart = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-11));
            var start = startDate ?? defaultStart;
            var end   = endDate   ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var rows = await _context.Bills
                .Where(b => b.PaymentStatus == "paid" && b.DateCreated >= start && b.DateCreated <= end)
                .GroupBy(b => new { b.DateCreated.Year, b.DateCreated.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = (long)g.Sum(b => b.TotalAmount)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return rows.Select(r => (r.Year, r.Month, r.Revenue));
        }

        public async Task<IEnumerable<(string CategoryName, long Revenue)>> GetRevenueByCategoryAsync(
            DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var defaultStart = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-11));
            var start = startDate ?? defaultStart;
            var end   = endDate   ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var rows = await _context.BillDetails
                .Include(bd => bd.IdProductNavigation)
                    .ThenInclude(p => p.IdCategoryNavigation)
                .Include(bd => bd.IdBillNavigation)
                .Where(bd => bd.IdBillNavigation.PaymentStatus == "paid"
                          && bd.IdBillNavigation.DateCreated >= start
                          && bd.IdBillNavigation.DateCreated <= end)
                .GroupBy(bd => bd.IdProductNavigation.IdCategoryNavigation!.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Revenue      = (long)g.Sum(bd => bd.TotalPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            return rows.Select(r => (r.CategoryName, r.Revenue));
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
