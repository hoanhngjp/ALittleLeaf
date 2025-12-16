using ALittleLeaf.Filters;
using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.Utils;
using ALittleLeaf.ViewModels;

using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Services.Order
{
    [CheckLogin]
    public class OrderService : IOrderService
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(AlittleLeafDecorContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public async Task<List<AddressList>> GetUserAddressesAsync(long userId)
        {
            return await _context.AddressLists
                .Where(a => a.IdUser == userId)
                .ToListAsync();
        }

        public async Task<AddressList> GetAddressDetailsAsync(int addressId)
        {
            return await _context.AddressLists.FirstOrDefaultAsync(a => a.AdrsId == addressId);
        }
        [CheckLogin]
        public async Task<Bill> CreateOrderFromSessionAsync(string paymentMethod, string paymentStatus)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            int adrsId;

            // Bắt đầu Transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Xử lý địa chỉ
                    if (Session.GetString("BillingAdrsId") == "new")
                    {
                        var newAddress = new AddressList
                        {
                            AdrsFullname = Session.GetString("BillingFullName"),
                            AdrsAddress = Session.GetString("BillingAddress"),
                            AdrsPhone = Session.GetString("BillingPhone"),
                            AdrsIsDefault = false,
                            IdUser = userId
                        };
                        _context.AddressLists.Add(newAddress);
                        await _context.SaveChangesAsync();
                        adrsId = newAddress.AdrsId;
                    }
                    else
                    {
                        adrsId = int.Parse(Session.GetString("BillingAdrsId"));
                    }

                    // 2. Lấy giỏ hàng
                    var cart = Session.GetObjectFromJson<List<CartItemViewModel>>("Cart");
                    if (cart == null || !cart.Any())
                    {
                        throw new Exception("Giỏ hàng trống.");
                    }

                    int totalAmount = cart.Sum(c => c.Quantity * c.ProductPrice);
                    string Note = Session.GetString("BillNote");

                    // 3. Tạo Bill
                    var bill = new Bill
                    {
                        IdUser = userId,
                        IdAdrs = adrsId,
                        DateCreated = DateOnly.FromDateTime(DateTime.Now),
                        TotalAmount = totalAmount,
                        PaymentMethod = paymentMethod,
                        PaymentStatus = paymentStatus,
                        IsConfirmed = false,
                        ShippingStatus = "not_fulfilled",
                        Note = Note,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Bills.Add(bill);
                    await _context.SaveChangesAsync();

                    var billDetails = new List<BillDetail>();
                    foreach (var item in cart)
                    {
                        billDetails.Add(new BillDetail
                        {
                            IdBill = bill.BillId,
                            IdProduct = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.ProductPrice,
                            TotalPrice = item.Quantity * item.ProductPrice,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                    }
                    _context.BillDetails.AddRange(billDetails);

                    await _context.SaveChangesAsync();

                    // 5. Commit Transaction
                    await transaction.CommitAsync();
                    return bill;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log lỗi ex.Message
                    throw new Exception("Lỗi khi tạo đơn hàng: " + ex.Message);
                }
            }
        }

        // --- LOGIC TRỪ KHO & XÓA SESSION ---
        public async Task FulfillOrderAsync(int billId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var bill = await _context.Bills
                        .Include(b => b.BillDetails)
                        .FirstOrDefaultAsync(b => b.BillId == billId);

                    if (bill == null) throw new Exception("Không tìm thấy đơn hàng.");

                    // 1. Trừ kho
                    foreach (var item in bill.BillDetails)
                    {
                        var product = await _context.Products.FindAsync(item.IdProduct);
                        if (product != null)
                        {
                            if (product.QuantityInStock < item.Quantity)
                            {
                                throw new Exception($"Sản phẩm {product.ProductName} không đủ hàng.");
                            }
                            product.QuantityInStock -= item.Quantity;
                        }
                    }

                    // 2. Cập nhật Bill (Nếu cần, ví dụ: IsConfirmed)
                    // (Logic này đã được xử lý trong PaymentCallbackVnpay)

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 3. Xóa Session (Chỉ khi mọi thứ thành công)
                    Session.Remove("BillingFullName");
                    Session.Remove("BillingAddress");
                    Session.Remove("BillingPhone");
                    Session.Remove("BillNote");
                    Session.Remove("BillingAdrsId");
                    Session.Remove("Cart");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log lỗi
                    throw; // Ném lỗi ra để Controller xử lý
                }
            }
        }
        
        public async Task<List<Bill>> GetOrderHistoryAsync(long userId)
        {
            return await _context.Bills
                .Where(b => b.IdUser == userId)
                .OrderByDescending(b => b.DateCreated)
                .ToListAsync();
        }

        public async Task<Bill> GetBillByIdAsync(int billId, long userId)
        {
            return await _context.Bills
                .Include(b => b.IdAdrsNavigation) // Lấy luôn địa chỉ giao hàng
                .FirstOrDefaultAsync(b => b.BillId == billId && b.IdUser == userId);
        }

        public async Task<List<OrderDetailViewModel>> GetBillDetailsAsync(int billId)
        {
            // Dùng async và ToListAsync()
            return await (from bd in _context.BillDetails
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
                          }).ToListAsync();
        }
    }
}
