using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Tests.MockData
{
    public static class DbMock
    {
        public static List<Category> GetCategories() => new()
        {
            new Category { CategoryId = 1, CategoryName = "Bếp",      CategoryParentId = null, CreatedAt = DateTime.Now },
            new Category { CategoryId = 2, CategoryName = "Cây cảnh", CategoryParentId = null, CreatedAt = DateTime.Now },
            new Category { CategoryId = 3, CategoryName = "Cốc, Ly",  CategoryParentId = null, CreatedAt = DateTime.Now },
        };

        public static List<User> GetUsers() => new()
        {
            new User
            {
                UserId = 1, UserEmail = "admin@test.com", UserPassword = "hashed_pass",
                UserFullname = "Administrator", UserRole = "Admin", UserIsActive = true,
                UserSex = true, UserBirthday = new DateOnly(1990, 1, 1), CreatedAt = DateTime.Now
            },
            new User
            {
                UserId = 2, UserEmail = "user@test.com", UserPassword = "hashed_pass",
                UserFullname = "Nguyen Van A", UserRole = "Customer", UserIsActive = true,
                UserSex = true, UserBirthday = new DateOnly(2000, 5, 15), CreatedAt = DateTime.Now
            },
            new User
            {
                UserId = 3, UserEmail = "locked@test.com", UserPassword = "hashed_pass",
                UserFullname = "Locked User", UserRole = "Customer", UserIsActive = false,
                UserSex = false, UserBirthday = new DateOnly(2002, 12, 12), CreatedAt = DateTime.Now
            },
        };

        public static List<Product> GetProducts() => new()
        {
            new Product
            {
                ProductId = 1, ProductName = "Ấm Tráng Men", ProductPrice = 50000,
                QuantityInStock = 100, IdCategory = 1, ProductDescription = "Ấm đẹp",
                IsOnSale = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            },
            new Product
            {
                ProductId = 2, ProductName = "Bếp Từ Cao Cấp", ProductPrice = 2500000,
                QuantityInStock = 10, IdCategory = 1, ProductDescription = "Bếp xịn",
                IsOnSale = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            },
            new Product
            {
                ProductId = 3, ProductName = "Cây Xương Rồng", ProductPrice = 20000,
                QuantityInStock = 5, IdCategory = 2, ProductDescription = "Cây đẹp",
                IsOnSale = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            },
            new Product
            {
                ProductId = 4, ProductName = "Sản phẩm cũ", ProductPrice = 10000,
                QuantityInStock = 0, IdCategory = 1, IsOnSale = false,
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            },
        };

        public static List<AddressList> GetAddressLists() => new()
        {
            new AddressList
            {
                AdrsId = 10, IdUser = 2, AdrsFullname = "Nguyen Van A",
                AdrsPhone = "0909000111", AdrsAddress = "123 Duong ABC, TP.HCM",
                AdrsIsDefault = true, CreatedAt = DateTime.Now
            },
        };

        public static List<Bill> GetBills() => new()
        {
            new Bill
            {
                BillId = 100, IdUser = 2, IdAdrs = 10,
                DateCreated = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                TotalAmount = 50000, PaymentMethod = "VNPAY", PaymentStatus = "paid",
                ShippingStatus = "fulfilled", IsConfirmed = true, UpdatedAt = DateTime.Now
            },
        };

        public static List<BillDetail> GetBillDetails() => new()
        {
            new BillDetail
            {
                BillDetailId = 1, IdBill = 100, IdProduct = 1,
                Quantity = 1, UnitPrice = 50000, TotalPrice = 50000, CreatedAt = DateTime.Now
            },
        };
    }
}
