using MetaKing.BackendServer.Data.Entities;
using MetaKing.ViewModels.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetaKing.BackendServer.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            // Seed Roles
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Admin User
            if (!userManager.Users.Any())
            {
                var admin = new UserModel
                {
                    UserName = "admin",
                    Email = "admin@metaking.com",
                    FullName = "Administrator",
                    Address = "123 Admin St",
                    City = "AdminCity",
                    District = "AdminDistrict",
                    Ward = "AdminWard",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Brands
            if (!context.Brands.Any())
            {
                context.Brands.AddRange(new List<BrandModel>
                {
                    new BrandModel { Name = "Apple", Description = "Apple Inc.", Slug = "apple", Status = StatusType.Active },
                    new BrandModel { Name = "Samsung", Description = "Samsung Electronics", Slug = "samsung", Status = StatusType.Active }
                });
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new List<CategoryModel>
                {
                    new CategoryModel { Name = "Phones", Description = "Mobile Phones", Slug = "phones", Status = StatusType.Active },
                    new CategoryModel { Name = "Laptops", Description = "Laptop Devices", Slug = "laptops", Status = StatusType.Active }
                });
            }

            // Seed Companies
            if (!context.Companies.Any())
            {
                context.Companies.AddRange(new List<CompanyModel>
                {
                    new CompanyModel { Name = "Tech Corp", Description = "Electronics Supplier", Slug = "tech-corp", Status = StatusType.Active }
                });
            }

            // Seed Colors
            if (!context.Colors.Any())
            {
                context.Colors.AddRange(new List<ColorModel>
                {
                    new ColorModel { Name = "Black" },
                    new ColorModel { Name = "White" }
                });
            }

            // Seed Versions
            if (!context.Versions.Any())
            {
                context.Versions.AddRange(new List<VersionModel>
                {
                    new VersionModel { Name = "64GB" },
                    new VersionModel { Name = "128GB" }
                });
            }

            // Seed Products
            if (!context.Products.Any())
            {
                context.Products.Add(new ProductModel
                {
                    Name = "iPhone 15",
                    Image = "iphone15.jpg",
                    Description = "Latest iPhone model",
                    ProductType = ProductType.Phone,
                    Slug = "iphone-15",
                    BrandId = 1,
                    CategoryId = 1,
                    CompanyId = 1,
                    Status = StatusType.Active
                });
            }

            await context.SaveChangesAsync();

            // Seed Product Variants
            if (!context.ProductVariants.Any())
            {
                context.ProductVariants.AddRange(new List<ProductVariantModel>
                {
                    new ProductVariantModel
                    {
                        ProductId = 1,
                        VersionId = 1,
                        ColorId = 1,
                        ImportPrice = 800,
                        Price = 1000,
                        Quantity = 50,
                        Sold = 0,
                    }
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
