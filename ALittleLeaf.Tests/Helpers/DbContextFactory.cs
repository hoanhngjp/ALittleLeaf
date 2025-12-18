using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Linq; // Nhớ dòng này để dùng .Any()

namespace ALittleLeaf.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static AlittleLeafDecorContext Create()
        {
            var options = new DbContextOptionsBuilder<AlittleLeafDecorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Mỗi test 1 DB mới
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging() // Giúp debug lỗi dữ liệu dễ hơn
                .Options;

            var context = new AlittleLeafDecorContext(options);

            // Bắt buộc phải tạo DB
            context.Database.EnsureCreated();

            // --- SEED DATA (NẠP DỮ LIỆU) ---
            // Kiểm tra xem đã có Categories chưa, nếu chưa thì nạp toàn bộ
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(DbMock.GetCategories());
                context.Products.AddRange(DbMock.GetProducts()); // <-- QUAN TRỌNG: Nạp sản phẩm
                context.BillDetails.AddRange(DbMock.GetBillDetails()); // <-- Nạp chi tiết đơn để test thống kê

                // Nạp Users nếu cần test Auth trong cùng context này
                // context.Users.AddRange(DbMock.GetUsers()); 

                context.SaveChanges(); // <-- BẮT BUỘC: Lưu lại thì DB mới có dữ liệu
            }

            return context;
        }

        public static void Destroy(AlittleLeafDecorContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}