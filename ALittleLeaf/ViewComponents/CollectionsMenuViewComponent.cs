using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.ViewComponents
{
    public class CollectionsMenuViewComponent : ViewComponent
    {
        private readonly AlittleLeafDecorContext db;
        public CollectionsMenuViewComponent(AlittleLeafDecorContext context) => db = context;
        public IViewComponentResult Invoke()
        {
            // Lấy tất cả các danh mục từ cơ sở dữ liệu
            var categories = db.Categories
                .Where(c => c.CategoryParentId == 0) // Lấy các danh mục cha
                .Select(c => new CollectionsMenuViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    SubCategories = db.Categories
                        .Where(sub => sub.CategoryParentId == c.CategoryId)
                        .Select(sub => new CollectionsMenuViewModel
                        {
                            CategoryId = sub.CategoryId,
                            CategoryName = sub.CategoryName
                        })
                        .ToList()
                })
                .ToList();

            // Trả về View với dữ liệu danh mục
            return View(categories);
        }
    }
}
