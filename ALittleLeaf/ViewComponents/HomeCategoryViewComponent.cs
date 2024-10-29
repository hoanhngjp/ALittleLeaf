using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.ViewComponents
{
    public class HomeCategoryViewComponent:ViewComponent
    {
        private readonly AlittleLeafDecorContext db;
        public HomeCategoryViewComponent(AlittleLeafDecorContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var categories = db.Categories
                .Where(c => c.CategoryParentId == 0) // Đảm bảo không có danh mục con
                .Select(c => new HomeCategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryImg = c.CategoryImg
                });

            return View(categories); // Trả về IQueryable
        }
    }
}
