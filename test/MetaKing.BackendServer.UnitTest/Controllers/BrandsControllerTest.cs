using MetaKing.BackendServer.Controllers;
using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.ViewModels.Enum;

namespace MetaKing.BackendServer.UnitTest.Controllers
{
    public class BrandsControllerTest
    {
        private readonly BrandsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public BrandsControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Brands.Add(new BrandModel
            {
                Name = "Test Brand",
                Description = "Description",
                Slug = "test-brand",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new BrandsController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetBrands_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<BrandViewModel>>("Brands"))
                .ReturnsAsync((List<BrandViewModel>)null);

            // Act
            var result = await _controller.GetBrands();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var Brands = Assert.IsAssignableFrom<List<BrandViewModel>>(okResult.Value);
            Assert.NotEmpty(Brands);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsBrand()
        {
            // Arrange
            var existing = _context.Brands.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var brand = Assert.IsType<BrandViewModel>(okResult.Value);
            Assert.Equal(existing.Id, brand.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostBrand_ValidRequest_ShouldCreateBrand()
        {
            var request = new BrandCreateRequest
            {
                Name = "New Brand",
                Description = "New Description",
                Slug = "new-brand",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostBrand(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutBrand_ExistingId_UpdatesBrand()
        {
            var brand = _context.Brands.First();
            var request = new BrandCreateRequest
            {
                Name = "Updated",
                Description = "Updated Description",
                Slug = "updated-slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutBrand(brand.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutBrand_NotFound_ReturnsNotFound()
        {
            var request = new BrandCreateRequest
            {
                Name = "Name",
                Description = "Desc",
                Slug = "slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutBrand(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBrand_ExistingId_DeletesBrand()
        {
            var brand = _context.Brands.First();
            var result = await _controller.DeleteBrand(brand.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedBrand = Assert.IsType<BrandViewModel>(okResult.Value);
            Assert.Equal(brand.Id, deletedBrand.Id);
        }

        [Fact]
        public async Task DeleteBrand_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteBrand(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetBrandsPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetBrandsPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<BrandViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
