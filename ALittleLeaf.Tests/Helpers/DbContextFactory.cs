using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Tests.MockData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ALittleLeaf.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static AlittleLeafDecorContext Create()
        {
            var options = new DbContextOptionsBuilder<AlittleLeafDecorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging()
                .Options;

            var context = new AlittleLeafDecorContext(options);
            context.Database.EnsureCreated();

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(DbMock.GetCategories());
                context.Products.AddRange(DbMock.GetProducts());
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

    /// <summary>
    /// Assertion helpers for Web API IActionResult responses.
    /// Replaces MVC-era ViewResult / RedirectToActionResult assertions.
    /// </summary>
    public static class ApiAssert
    {
        public static T OkValue<T>(IActionResult result)
        {
            var ok = Assert.IsType<OkObjectResult>(result);
            return Assert.IsType<T>(ok.Value);
        }

        public static OkObjectResult IsOk(IActionResult result)
            => Assert.IsType<OkObjectResult>(result);

        public static BadRequestObjectResult IsBadRequest(IActionResult result)
            => Assert.IsType<BadRequestObjectResult>(result);

        public static NotFoundObjectResult IsNotFound(IActionResult result)
            => Assert.IsType<NotFoundObjectResult>(result);

        public static UnauthorizedObjectResult IsUnauthorized(IActionResult result)
            => Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
