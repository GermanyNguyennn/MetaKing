using MetaKing.BackendServer.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MetaKing.BackendServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------- USER ----------------
            modelBuilder.Entity<UserModel>()
                .HasMany<OrderModel>()
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany<CartModel>()
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- PRODUCT ----------------
            modelBuilder.Entity<ProductModel>()
                .HasMany(x => x.ProductVariants)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductModel>()
                .HasMany(x => x.OrderDetails)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                .HasOne(x => x.ProductDetailPhones)
                .WithOne(x => x.Product)
                .HasForeignKey<ProductDetailPhoneModel>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductModel>()
                .HasOne(x => x.ProductDetailLaptops)
                .WithOne(x => x.Product)
                .HasForeignKey<ProductDetailLaptopModel>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductModel>()
               .HasOne(x => x.ProductDetailWatches)
               .WithOne(x => x.Product)
               .HasForeignKey<ProductDetailWatchModel>(x => x.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            // ---------------- VARIANTS ----------------
            modelBuilder.Entity<ProductVariantModel>()
                .HasMany(x => x.OrderDetails)
                .WithOne(x => x.ProductVariant)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- ORDER & ORDER DETAIL ----------------
            modelBuilder.Entity<OrderModel>()
                .HasMany(x => x.OrderDetails)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- CATEGORY / BRAND / COMPANY ----------------
            modelBuilder.Entity<CategoryModel>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BrandModel>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Brand)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompanyModel>()
                .HasMany(x => x.Products)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- COUPON ----------------
            modelBuilder.Entity<OrderModel>()
                .HasOne(x => x.Coupon)
                .WithMany()
                .HasForeignKey(x => x.CouponId)
                .OnDelete(DeleteBehavior.SetNull);

            // ---------------- COLOR / VERSION ----------------
            modelBuilder.Entity<ColorModel>()
                .HasMany<ProductVariantModel>()
                .WithOne(x => x.Color)
                .HasForeignKey(x => x.ColorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VersionModel>()
                .HasMany<ProductVariantModel>()
                .WithOne(x => x.Version)
                .HasForeignKey(x => x.VersionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- CART ----------------
            modelBuilder.Entity<CartModel>()
                .HasOne(x => x.ProductVariant)
                .WithMany()
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- PAYMENT ----------------
            modelBuilder.Entity<MoMoModel>()
                .HasIndex(x => x.OrderId)
                .IsUnique(false);

            modelBuilder.Entity<VNPayModel>()
                .HasIndex(x => x.OrderId)
                .IsUnique(false);
        }

        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ColorModel> Colors { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<MoMoModel> MoMos { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<ProductDetailPhoneModel> ProductDetailPhones { get; set; }
        public DbSet<ProductDetailLaptopModel> ProductDetailLaptops { get; set; }
        public DbSet<ProductDetailWatchModel> ProductDetailWatches { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductVariantModel> ProductVariants { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<VersionModel> Versions { get; set; }
        public DbSet<VNPayModel> VNPays { get; set; }  
    }
}
