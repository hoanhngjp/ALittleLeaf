using ALittleLeaf.Filters;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    [CheckLogin]
    public class OrderDetailController : SiteBaseController
    {
        private readonly AlittleLeafDecorContext _context;

        public OrderDetailController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int billId)
        {
            var billDetails = (from bd in _context.BillDetails
                               join p in _context.Products on bd.IdProduct equals p.ProductId
                               join pi in _context.ProductImages on p.ProductId equals pi.IdProduct
                               where bd.IdBill == billId && pi.IsPrimary == true
                               select new OrderDetailViewModel
                               {
                                   BillDetailId = bd.BillDetailId,
                                   IdBill = bd.IdBill,
                                   IdProduct = bd.IdProduct,
                                   IdCategory = p.IdCategory,
                                   Quantity = bd.Quantity,
                                   UnitPrice = bd.UnitPrice,
                                   TotalPrice = bd.TotalPrice,
                                   ProductName = p.ProductName,
                                   ProductImg = pi.ImgName // Chỉ lấy hình ảnh chính
                               }).ToList();

            return View(billDetails);
        }
    }
}
