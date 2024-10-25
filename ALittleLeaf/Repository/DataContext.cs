using ALittleLeaf.Models;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<AddressListModel> AddressLists { get; set; }
        public DbSet<BillModel> Bills { get; set; }
        public DbSet<BillDetailModel> BillDetails { get; set; }
        public DbSet<CategoryModel> Categorys { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
