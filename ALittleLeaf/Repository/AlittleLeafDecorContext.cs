using System;
using System.Collections.Generic;
using ALittleLeaf.Models;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Repository
{
    public partial class AlittleLeafDecorContext : DbContext
    {
        public AlittleLeafDecorContext()
        {
        }

        public AlittleLeafDecorContext(DbContextOptions<AlittleLeafDecorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressList> AddressLists { get; set; }

        public virtual DbSet<Bill> Bills { get; set; }

        public virtual DbSet<BillDetail> BillDetails { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressList>(entity =>
            {
                entity.HasKey(e => e.AdrsId).HasName("PK__Address___3D4B39253A6153A6");

                entity.ToTable("Address_List", tb => tb.HasTrigger("trg_UpdateAddressList"));

                entity.Property(e => e.AdrsId).HasColumnName("adrs_id");
                entity.Property(e => e.AdrsAddress)
                    .HasMaxLength(255)
                    .HasColumnName("adrs_address");
                entity.Property(e => e.AdrsFullname)
                    .HasMaxLength(255)
                    .HasColumnName("adrs_fullname");
                entity.Property(e => e.AdrsIsDefault).HasColumnName("adrs_isDefault");
                entity.Property(e => e.AdrsPhone)
                    .HasMaxLength(10)
                    .HasColumnName("adrs_phone");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.AddressLists)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Address_L__id_us__403A8C7D");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.BillId).HasName("PK__Bill__D706DDB3FB76D27B");

                entity.ToTable("Bill", tb => tb.HasTrigger("trg_UpdateBill"));

                entity.Property(e => e.BillId).HasColumnName("bill_id");
                entity.Property(e => e.DateCreated).HasColumnName("date_created");
                entity.Property(e => e.IdAdrs).HasColumnName("id_adrs");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.IsConfirmed).HasColumnName("is_confirmed");
                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method");
                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(50)
                    .HasColumnName("payment_status");
                entity.Property(e => e.ShippingStatus)
                    .HasMaxLength(50)
                    .HasColumnName("shipping_status");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdAdrsNavigation).WithMany(p => p.Bills)
                    .HasForeignKey(d => d.IdAdrs)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill__id_adrs__45F365D3");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Bills)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill__updated_at__44FF419A");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => e.BillDetailId).HasName("PK__Bill_Det__3BCDAFBBC3AFB005");

                entity.ToTable("Bill_Detail", tb => tb.HasTrigger("trg_UpdateBillDetail"));

                entity.Property(e => e.BillDetailId).HasColumnName("bill_detail_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdBill).HasColumnName("id_bill");
                entity.Property(e => e.IdProduct).HasColumnName("id_product");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.TotalPrice).HasColumnName("total_price");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdBillNavigation).WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.IdBill)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill_Deta__id_bi__5441852A");

                entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill_Deta__id_pr__5535A963");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B4D7980457");

                entity.ToTable("Category", tb => tb.HasTrigger("trg_UpdateCategory"));

                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CategoryImg)
                    .HasMaxLength(255)
                    .HasColumnName("category_img");
                entity.Property(e => e.CategoryName)
                    .HasMaxLength(255)
                    .HasColumnName("category_name");
                entity.Property(e => e.CategoryParentId).HasColumnName("category_parent_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
                entity.HasOne(d => d.ParentCategory)
          .WithMany(p => p.SubCategories)
          .HasForeignKey(d => d.CategoryParentId)
          .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId).HasName("PK__Product__47027DF5D9D271A0");

                entity.ToTable("Product", tb => tb.HasTrigger("trg_UpdateProduct"));

                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdCategory).HasColumnName("id_category");
                entity.Property(e => e.IsOnSale).HasColumnName("is_onSale");
                entity.Property(e => e.ProductDescription).HasColumnName("product_description");
                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("product_name");
                entity.Property(e => e.ProductPrice).HasColumnName("product_price");
                entity.Property(e => e.QuantityInStock).HasColumnName("quantity_in_stock");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__id_cate__4F7CD00D");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ImgId).HasName("PK__Product___6F16A71C6F40848C");

                entity.ToTable("Product_Images", tb => tb.HasTrigger("trg_UpdateProductImages"));

                entity.Property(e => e.ImgId).HasColumnName("img_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdProduct).HasColumnName("id_product");
                entity.Property(e => e.ImgName)
                    .HasMaxLength(255)
                    .HasColumnName("img_name");
                entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product_I__id_pr__5AEE82B9");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370FE2FFFB58");

                entity.ToTable("User", tb => tb.HasTrigger("trg_UpdateUser"));

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
                entity.Property(e => e.UserBirthday).HasColumnName("user_birthday");
                entity.Property(e => e.UserEmail)
                    .HasMaxLength(255)
                    .HasColumnName("user_email");
                entity.Property(e => e.UserFullname)
                    .HasMaxLength(255)
                    .HasColumnName("user_fullname");
                entity.Property(e => e.UserIsActive)
                    .HasDefaultValue(true)
                    .HasColumnName("user_isActive");
                entity.Property(e => e.UserPassword)
                    .HasMaxLength(255)
                    .HasColumnName("user_password");
                entity.Property(e => e.UserRole)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("customer")
                    .HasColumnName("user_role");
                entity.Property(e => e.UserSex).HasColumnName("user_sex");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken"); // Tên bảng
                entity.HasKey(e => e.Id); // Khóa chính

                entity.Property(e => e.Token).IsRequired();
                entity.Property(e => e.JwtId).IsRequired();

                // Cấu hình khóa ngoại
                entity.HasOne(d => d.User)
                      .WithMany() // Một User có nhiều RefreshToken
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_RefreshToken_User_UserId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}