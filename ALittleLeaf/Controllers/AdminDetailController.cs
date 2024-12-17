using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Controllers
{
    public class AdminDetailController : Controller
    {
        private readonly AlittleLeafDecorContext _context;

        public AdminDetailController(AlittleLeafDecorContext context)
        {
            _context = context;
        }
        public IActionResult Index(int billId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            // Lấy thông tin hóa đơn từ bảng Bill
            var bill = _context.Bills
                .Where(b => b.BillId == billId)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    CustomerName = b.IdUserNavigation.UserFullname,
                    CustomerPhone = b.IdAdrsNavigation.AdrsPhone,
                    ShippingAddress = b.IdAdrsNavigation.AdrsAddress,
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    PaymentStatus = b.PaymentStatus,
                    IsConfirmed = b.IsConfirmed,
                    Note = b.Note,
                    ShippingStatus = b.ShippingStatus,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefault();

            // Lấy chi tiết đơn hàng từ BillDetails và Products
            var orderDetails = (from bd in _context.BillDetails
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
                                    ProductImg = pi.ImgName
                                }).ToList();

            // Tạo đối tượng AdminBillDetailViewModel kết hợp thông tin hóa đơn và chi tiết đơn hàng
            var model = new AdminBillDetailViewModel
            {
                Bill = bill,
                OrderDetails = orderDetails
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateBillStatus(int BillId, bool IsConfirmed, string PaymentStatus, string ShippingStatus)
        {
            var bill = _context.Bills.FirstOrDefault(b => b.BillId == BillId);

            if (bill != null)
            {
                bill.IsConfirmed = IsConfirmed;
                bill.PaymentStatus = PaymentStatus;
                bill.ShippingStatus = ShippingStatus;
                bill.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { billId = BillId });
        }
    }
}
