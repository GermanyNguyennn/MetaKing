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
    public class CategoriesControllerTest
    {
        private readonly CategoriesController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public CategoriesControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Categories.Add(new CategoryModel
            {
                Name = "Test Category",
                Description = "Description",
                Slug = "test-category",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new CategoriesController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<CategoryViewModel>>("Categories"))
                .ReturnsAsync((List<CategoryViewModel>)null);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<List<CategoryViewModel>>(okResult.Value);
            Assert.NotEmpty(categories);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsCategory()
        {
            // Arrange
            var existing = _context.Categories.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var category = Assert.IsType<CategoryViewModel>(okResult.Value);
            Assert.Equal(existing.Id, category.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostCategory_ValidRequest_ShouldCreateCategory()
        {
            var request = new CategoryCreateRequest
            {
                Name = "New Category",
                Description = "New Description",
                Slug = "new-category",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostCategory(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutCategory_ExistingId_UpdatesCategory()
        {
            var category = _context.Categories.First();
            var request = new CategoryCreateRequest
            {
                Name = "Updated",
                Description = "Updated Description",
                Slug = "updated-slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutCategory(category.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutCategory_NotFound_ReturnsNotFound()
        {
            var request = new CategoryCreateRequest
            {
                Name = "Name",
                Description = "Desc",
                Slug = "slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutCategory(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCategory_ExistingId_DeletesCategory()
        {
            var category = _context.Categories.First();
            var result = await _controller.DeleteCategory(category.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedCategory = Assert.IsType<CategoryViewModel>(okResult.Value);
            Assert.Equal(category.Id, deletedCategory.Id);
        }

        [Fact]
        public async Task DeleteCategory_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteCategory(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetCategoriesPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetCategoriesPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<CategoryViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
