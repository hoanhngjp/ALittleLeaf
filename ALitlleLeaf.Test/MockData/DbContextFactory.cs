using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics; // <-- Thêm namespace này
using System;

namespace ALittleLeaf.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static AlittleLeafDecorContext Create()
        {
            var options = new DbContextOptionsBuilder<AlittleLeafDecorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // <-- DÒNG QUAN TRỌNG: Bỏ qua lỗi Transaction
                .Options;

            var context = new AlittleLeafDecorContext(options);

            context.Database.EnsureCreated();

            // Seeding data (nếu cần)
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(DbMock.GetCategories());
                // context.Users.AddRange(DbMock.GetUsers()); // Tạm thời comment dòng này để tránh conflict dữ liệu với từng test case riêng lẻ
                context.SaveChanges();
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