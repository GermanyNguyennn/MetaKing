using MetaKing.BackendServer.Controllers;
using MetaKing.BackendServer.Data.Entities;
using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MetaKing.ViewModels;
using MetaKing.BackendServer.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MetaKing.BackendServer.UnitTest.Controllers
{
    public class ProductsControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        public ProductsControllerTest()
        {
            _context = CreateDbContext();
            _cacheServiceMock = new Mock<ICacheService>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(Directory.GetCurrentDirectory());

            SeedDependencies(_context); // ← Bắt buộc: seed Brand, Category, Company có ID = 1
        }

        private void SeedDependencies(ApplicationDbContext db)
        {
            if (!db.Categories.Any())
            {
                db.Categories.Add(new CategoryModel
                {
                    Id = 1,
                    Name = "Điện thoại",
                    Description = "Danh mục điện thoại",
                    Slug = "dien-thoai"
                });
            }

            if (!db.Brands.Any())
            {
                db.Brands.Add(new BrandModel
                {
                    Id = 1,
                    Name = "Apple",
                    Description = "Thương hiệu Apple",
                    Slug = "apple"
                });
            }

            if (!db.Companies.Any())
            {
                db.Companies.Add(new CompanyModel
                {
                    Id = 1,
                    Name = "MetaKing Co.",
                    Description = "Công ty MetaKing",
                    Slug = "metaking"
                });
            }

            db.SaveChanges();
        }


        private ProductModel CreateValidProduct(string name = "Test Product", ProductType type = ProductType.Phone)
        {
            return new ProductModel
            {
                Name = name,
                Description = "Test Description",
                Image = "test.jpg",
                Slug = name.ToLower().Replace(" ", "-"),
                ProductType = type,
                BrandId = 1,
                CategoryId = 1,
                CompanyId = 1,
                Status = StatusType.Active,
                CreatedDate = DateTime.Now
            };
        }

        private ProductsController CreateController(ApplicationDbContext? context = null)
        {
            context ??= CreateDbContext();
            SeedDependencies(context);

            return new ProductsController(context, _cacheServiceMock.Object, _webHostEnvironmentMock.Object);
        }

        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetProducts_NoCache_ReturnsData()
        {
            _cacheServiceMock.Setup(x => x.GetAsync<List<ProductViewModel>>("Products")).ReturnsAsync((List<ProductViewModel>)null);

            _context.Products.Add(CreateValidProduct("iPhone"));
            await _context.SaveChangesAsync();

            var controller = CreateController(_context);
            var result = await controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(okResult.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetById_ProductExists_ReturnsOk()
        {
            var product = CreateValidProduct("Laptop", ProductType.Laptop);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var controller = CreateController(_context);
            var result = await controller.GetById(product.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var viewModel = Assert.IsType<ProductViewModel>(okResult.Value);
            Assert.Equal("Laptop", viewModel.Name);
        }

        [Fact]
        public async Task GetById_ProductNotFound_ReturnsNotFound()
        {
            var controller = CreateController();
            var result = await controller.GetById(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiNotFoundResponse>(notFound.Value);
            Assert.Contains("not found", response.Message.ToLower());
        }


        [Fact]
        public async Task GetDetailByProductType_Phone_ReturnsDetail()
        {
            var product = CreateValidProduct("Phone", ProductType.Phone);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.ProductDetailPhones.Add(new ProductDetailPhoneModel
            {
                ProductId = product.Id,
                ScreenSize = "6.1",
                ChipSet = "A15",
                CategoryId = 1,
                BrandId = 1,
                CompanyId = 1
            });
            await _context.SaveChangesAsync();

            var controller = CreateController(_context);
            var result = await controller.GetDetailByProductType(product.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var detail = Assert.IsType<ProductDetailPhoneModel>(ok.Value);
            Assert.Equal("A15", detail.ChipSet);
        }

        [Fact]
        public async Task GetDetailByProductType_Laptop_ReturnsDetail()
        {
            var product = CreateValidProduct("Laptop", ProductType.Laptop);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.ProductDetailLaptops.Add(new ProductDetailLaptopModel
            {
                ProductId = product.Id,
                CPUType = "Intel i7",
                CategoryId = 1,
                BrandId = 1,
                CompanyId = 1
            });
            await _context.SaveChangesAsync();

            var controller = CreateController(_context);
            var result = await controller.GetDetailByProductType(product.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var detail = Assert.IsType<ProductDetailLaptopModel>(ok.Value);
            Assert.Equal("Intel i7", detail.CPUType);
        }

        [Fact]
        public async Task GetDetailByProductType_Watch_ReturnsDetail()
        {
            var product = CreateValidProduct("Watch", ProductType.Watch);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.ProductDetailWatches.Add(new ProductDetailWatchModel
            {
                ProductId = product.Id,
                BatteryLife = "2 days",
                CategoryId = 1,
                BrandId = 1,
                CompanyId = 1
            });
            await _context.SaveChangesAsync();

            var controller = CreateController(_context);
            var result = await controller.GetDetailByProductType(product.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var detail = Assert.IsType<ProductDetailWatchModel>(ok.Value);
            Assert.Equal("2 days", detail.BatteryLife);
        }

        [Fact]
        public async Task GetDetailByProductType_UnsupportedType_ReturnsBadRequest()
        {
            var product = CreateValidProduct("Tablet", ProductType.Tablet);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var controller = CreateController(_context);
            var result = await controller.GetDetailByProductType(product.Id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<string>(badRequest.Value);
            Assert.Contains("unsupported", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task PostProductDetail_InvalidProductId_ReturnsNotFound()
        {
            var detail = new ProductDetailPhoneModel
            {
                CPUType = "Test",
                BrandId = 1,
                CategoryId = 1,
                CompanyId = 1
            };
            var json = JsonSerializer.Serialize(detail);

            var controller = CreateController();
            var result = await controller.PostProductDetail(999, JsonDocument.Parse(json).RootElement);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiNotFoundResponse>(notFound.Value);
            Assert.Contains("not found", response.Message.ToLower());
        }


        [Fact]
        public async Task PostProductDetail_Phone_Valid()
        {
            var product = CreateValidProduct("Phone", ProductType.Phone);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var detail = new ProductDetailPhoneModel
            {
                CPUType = "Apple A18 Pro",
                CategoryId = 1,
                BrandId = 1,
                CompanyId = 1
            };
            var json = JsonSerializer.Serialize(detail);

            var controller = CreateController(_context);
            var result = await controller.PostProductDetail(product.Id, JsonDocument.Parse(json).RootElement);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiSuccessResponse>(okResult.Value);
            Assert.Equal("Detail added successfully.", response.Message);
        }

        [Fact]
        public async Task PostProductDetail_Laptop_Valid()
        {
            var product = CreateValidProduct("Laptop", ProductType.Laptop);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var detail = new ProductDetailLaptopModel
            {
                CPUType = "Ryzen 5",
                CategoryId = 1,
                BrandId = 1,
                CompanyId = 1
            };
            var json = JsonSerializer.Serialize(detail);

            var controller = CreateController(_context);
            var result = await controller.PostProductDetail(product.Id, JsonDocument.Parse(json).RootElement);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiSuccessResponse>(okResult.Value);
            Assert.Equal("Detail added successfully.", response.Message);
        }


        [Fact]
        public async Task PostProductDetail_Watch_Valid()
        {
            var product = CreateValidProduct("Watch", ProductType.Watch);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var detail = new ProductDetailWatchModel
            {
                BatteryLife = "3 days",
                CategoryId = 1,
                BrandId = 1,
                CompanyId = 1
            };
            var json = JsonSerializer.Serialize(detail);

            var controller = CreateController(_context);
            var result = await controller.PostProductDetail(product.Id, JsonDocument.Parse(json).RootElement);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiSuccessResponse>(okResult.Value);
            Assert.Equal("Detail added successfully.", response.Message);
        }

        [Fact]
        public async Task PostProduct_ValidInput_Success()
        {
            var mockFile = new Mock<IFormFile>();
            var content = "Fake Image";
            var fileName = "image.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(ms.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken token) =>
            {
                return ms.CopyToAsync(stream);
            });

            var request = new ProductCreateRequest
            {
                Name = "Product A",
                ImageUpload = mockFile.Object,
                Description = "Test Desc",
                ProductType = ProductType.Phone,
                Slug = "product-a",
                BrandId = 1,
                CategoryId = 1,
                CompanyId = 1,
                CreatedDate = DateTime.Now
            };

            var controller = CreateController();
            var result = await controller.PostProduct(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var product = Assert.IsType<ProductCreateRequest>(created.Value);
            Assert.Equal("Product A", product.Name);
        }

        [Fact]
        public async Task PostProduct_InvalidModelState_ReturnBadRequest()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("Name", "Name is required");

            var request = new ProductCreateRequest();

            var result = await controller.PostProduct(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequest.Value);
        }

        [Fact]
        public async Task PutProduct_ProductNotFound_ReturnNotFound()
        {
            var controller = CreateController();
            var request = new ProductCreateRequest { Name = "Test" };

            var result = await controller.PutProduct(999, request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiNotFoundResponse>(notFound.Value);
            Assert.Contains("not found", response.Message.ToLower());
        }

        [Fact]
        public async Task PutProduct_ValidInput_ReturnNoContent()
        {
            var db = CreateDbContext();
            SeedDependencies(db);

            var product = CreateValidProduct("Old Name");
            db.Products.Add(product);
            db.SaveChanges();

            var controller = CreateController(db);
            var request = new ProductCreateRequest
            {
                Name = "Updated Name",
                Slug = "updated-name",
                Description = "Updated Description",
                ProductType = ProductType.Phone,
                BrandId = 1,
                CategoryId = 1,
                CompanyId = 1,
                CreatedDate = DateTime.Now
            };

            var result = await controller.PutProduct(product.Id, request);

            Assert.IsType<NoContentResult>(result);
            var updated = db.Products.First(p => p.Id == product.Id);
            Assert.Equal("Updated Name", updated.Name);
        }

        [Fact]
        public async Task DeleteProduct_ValidId_ReturnOk()
        {
            var db = CreateDbContext();
            SeedDependencies(db);

            var product = CreateValidProduct("Delete Me");
            db.Products.Add(product);
            db.SaveChanges();

            var controller = CreateController(db);
            var result = await controller.DeleteProduct(product.Id);

            var ok = Assert.IsType<OkObjectResult>(result);

            // Kiểm tra trả về ProductViewModel
            var viewModel = Assert.IsType<ProductViewModel>(ok.Value);
            Assert.Equal("Delete Me", viewModel.Name); // hoặc ID...

            // Kiểm tra sản phẩm đã bị xoá khỏi DB
            Assert.False(db.Products.Any(p => p.Id == product.Id));
        }

        [Fact]
        public async Task DeleteProduct_ProductNotFound_ReturnsNotFound()
        {
            var controller = CreateController();
            var result = await controller.DeleteProduct(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiNotFoundResponse>(notFound.Value);
            Assert.Contains("not found", response.Message.ToLower());
        }

        [Fact]
        public async Task GetProductsPaging_WithFilter_ReturnFiltered()
        {
            var db = CreateDbContext();
            SeedDependencies(db);

            db.Products.AddRange(new List<ProductModel>
            {
                CreateValidProduct("iPhone"),
                CreateValidProduct("Samsung"),
                CreateValidProduct("iPad"),
            });
            db.SaveChanges();

            var controller = CreateController(db);
            var result = await controller.GetProductsPaging("iPhone", 1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<ProductViewModel>>(okResult.Value);
            Assert.Single(pagination.Items);
            Assert.Equal("iPhone", pagination.Items[0].Name);
        }       

        
    }
}