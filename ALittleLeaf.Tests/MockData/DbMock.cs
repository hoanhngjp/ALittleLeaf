using System;
using System.Collections.Generic;
using ALittleLeaf.Models; // Đảm bảo namespace này trỏ đúng về Project chính của bạn

namespace ALittleLeaf.Tests.MockData
{
    public static class DbMock
    {
        // 1. Mock Danh mục (Category)
        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Bếp",
                    CategoryParentId = null,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Cây cảnh",
                    CategoryParentId = null,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Cốc, Ly",
                    CategoryParentId = null,
                    CreatedAt = DateTime.Now
                }
            };
        }

        // 2. Mock Người dùng (User)
        // Lưu ý: Model User của bạn không có LockoutEnd/AccessFailedCount (Identity mặc định),
        // nên mình dùng UserIsActive để giả lập trạng thái khóa.
        public static List<User> GetUsers()
        {
            return new List<User>
            {
                // User 1: Admin
                new User
                {
                    UserId = 1,
                    UserEmail = "admin@test.com",
                    UserPassword = "hashed_pass",
                    UserFullname = "Administrator",
                    UserRole = "Admin",
                    UserIsActive = true,
                    UserSex = true,
                    UserBirthday = new DateOnly(1990, 1, 1),
                    CreatedAt = DateTime.Now
                },
                // User 2: Customer thường
                new User
                {
                    UserId = 2,
                    UserEmail = "user@test.com",
                    UserPassword = "hashed_pass",
                    UserFullname = "Nguyen Van A",
                    UserRole = "Customer",
                    UserIsActive = true,
                    UserSex = true,
                    UserBirthday = new DateOnly(2000, 5, 15),
                    CreatedAt = DateTime.Now
                },
                // User 3: Tài khoản bị khóa (UserIsActive = false)
                new User
                {
                    UserId = 3,
                    UserEmail = "locked@test.com",
                    UserPassword = "hashed_pass",
                    UserFullname = "Locked User",
                    UserRole = "Customer",
                    UserIsActive = false, // Giả lập bị khóa
                    UserSex = false,
                    UserBirthday = new DateOnly(2002, 12, 12),
                    CreatedAt = DateTime.Now
                }
            };
        }

        // 3. Mock Sản phẩm (Product)
        public static List<Product> GetProducts()
        {
            return new List<Product>
            {
                // SP 1: Dùng để test Tìm kiếm "Ấm", và Detail (ID=1)
                new Product
                {
                    ProductId = 1,
                    ProductName = "Ấm Tráng Men",
                    ProductPrice = 50000,
                    QuantityInStock = 100,
                    IdCategory = 1, // Bếp
                    ProductDescription = "Ấm đẹp",
                    IsOnSale = true, // <--- BẮT BUỘC TRUE
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                // SP 2: Dùng để test Lọc giá (> 2tr) và Tìm kiếm "Bếp"
                new Product
                {
                    ProductId = 2,
                    ProductName = "Bếp Từ Cao Cấp",
                    ProductPrice = 2500000, // Giá > 2.000.000
                    QuantityInStock = 10,
                    IdCategory = 1, // Bếp
                    ProductDescription = "Bếp xịn",
                    IsOnSale = true, // <--- QUAN TRỌNG: Phải là TRUE thì Service mới lấy
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                // SP 3: Dùng để test Lọc danh mục (Cây cảnh)
                new Product
                {
                    ProductId = 3,
                    ProductName = "Cây Xương Rồng",
                    ProductPrice = 20000,
                    QuantityInStock = 5,
                    IdCategory = 2, // Cây cảnh
                    ProductDescription = "Cây đẹp",
                    IsOnSale = true, // <--- BẮT BUỘC TRUE
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                // SP 4: SP này đã ẩn, dùng để test xem Service có lọc đúng không (nếu cần)
                new Product
                {
                    ProductId = 4,
                    ProductName = "Sản phẩm cũ",
                    ProductPrice = 10000,
                    QuantityInStock = 0,
                    IdCategory = 1,
                    IsOnSale = false, // <--- Cái này False thì Service sẽ không tìm thấy (Đúng logic)
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
        }
        // 4. Mock Địa chỉ (AddressList) - Cần thiết để tạo Bill
        public static List<AddressList> GetAddressLists()
        {
            return new List<AddressList>
            {
                new AddressList
                {
                    AdrsId = 10,
                    IdUser = 2, // Của Customer
                    AdrsFullname = "Nguyen Van A",
                    AdrsPhone = "0909000111",
                    AdrsAddress = "123 Duong ABC, TP.HCM",
                    AdrsIsDefault = true,
                    CreatedAt = DateTime.Now
                }
            };
        }

        // 5. Mock Đơn hàng (Bill) & Chi tiết (BillDetail)
        public static List<Bill> GetBills()
        {
            var bills = new List<Bill>
            {
                new Bill
                {
                    BillId = 100,
                    IdUser = 2, // User Customer
                    IdAdrs = 10,
                    DateCreated = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    TotalAmount = 50000,
                    PaymentMethod = "VNPAY",
                    PaymentStatus = "Paid",
                    ShippingStatus = "Delivered",
                    IsConfirmed = true,
                    UpdatedAt = DateTime.Now
                }
            };
            return bills;
        }

        // 6. Mock Bill Details (Nếu cần test chi tiết đơn hàng)
        public static List<BillDetail> GetBillDetails()
        {
            return new List<BillDetail>
            {
                new BillDetail
                {
                    BillDetailId = 1,
                    IdBill = 100,
                    IdProduct = 1, // Ấm Tráng Men
                    Quantity = 1,
                    UnitPrice = 50000,
                    TotalPrice = 50000,
                    CreatedAt = DateTime.Now
                }
            };
        }
    }
}