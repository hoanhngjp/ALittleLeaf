using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.ViewComponents
{
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly AlittleLeafDecorContext _context;

        public RelatedProductsViewComponent(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId, int idCategory)
        {
            // Lấy ra danh sách sản phẩm cùng category nhưng không bao gồm sản phẩm hiện tại
            var relatedProducts = await _context.Products
                .Where(p => p.IdCategory == idCategory && p.ProductId != productId)
                .OrderBy(r => Guid.NewGuid()) // Lấy ngẫu nhiên
                .Take(10) // Lấy 10 sản phẩm
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    IdCategory = p.IdCategory,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductImg = p.ProductImages.FirstOrDefault(i => i.IsPrimary).ImgName
                })
                .ToListAsync();

            return View(relatedProducts);
        }
    }
}
