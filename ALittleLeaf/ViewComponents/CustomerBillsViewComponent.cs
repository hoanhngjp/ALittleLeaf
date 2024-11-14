using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.ViewComponents
{
    public class CustomerBillsViewComponent : ViewComponent
    {
        private readonly AlittleLeafDecorContext db;
        public CustomerBillsViewComponent(AlittleLeafDecorContext context) => db = context;
        public async Task<IViewComponentResult> InvokeAsync(long userId)
        {
            var bills = await db.Bills
                .Where(b => b.IdUser == userId)
                .Select(b => new BillViewModel
                {
                    BillId = b.BillId,
                    DateCreated = b.DateCreated,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    PaymentStatus = b.PaymentStatus,
                    IsConfirmed = b.IsConfirmed,
                    ShippingStatus = b.ShippingStatus
                })
                .ToListAsync();

            if (!bills.Any())
            {
                ViewBag.NoBills = "No bills";
            }

            return View(bills);  // Trả về view với danh sách hóa đơn
        }
    }
}
