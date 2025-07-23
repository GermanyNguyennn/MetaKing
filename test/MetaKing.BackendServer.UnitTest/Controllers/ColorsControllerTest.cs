using MetaKing.BackendServer.Controllers;
using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels.Enum;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.BackendServer.Data.Entities;

namespace MetaKing.BackendServer.UnitTest.Controllers
{
    public class ColorsControllerTest
    {
        private readonly ColorsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public ColorsControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Colors.Add(new ColorModel
            {
                Name = "Test Color",
                Description = "Description",
                Slug = "test-color",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new ColorsController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetColors_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<ColorViewModel>>("Colors"))
                .ReturnsAsync((List<ColorViewModel>)null);

            // Act
            var result = await _controller.GetColors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var colors = Assert.IsAssignableFrom<List<ColorViewModel>>(okResult.Value);
            Assert.NotEmpty(colors);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsColor()
        {
            // Arrange
            var existing = _context.Colors.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var color = Assert.IsType<ColorViewModel>(okResult.Value);
            Assert.Equal(existing.Id, color.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostColor_ValidRequest_ShouldCreateColor()
        {
            var request = new ColorCreateRequest
            {
                Name = "New Color",
                Description = "New Description",
                Slug = "new-color",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostColor(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutColor_ExistingId_UpdatesColor()
        {
            var color = _context.Colors.First();
            var request = new ColorCreateRequest
            {
                Name = "Updated",
                Description = "Updated Description",
                Slug = "updated-slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutColor(color.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutColor_NotFound_ReturnsNotFound()
        {
            var request = new ColorCreateRequest
            {
                Name = "Name",
                Description = "Desc",
                Slug = "slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutColor(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteColor_ExistingId_DeletesColor()
        {
            var color = _context.Colors.First();
            var result = await _controller.DeleteColor(color.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedColor = Assert.IsType<ColorViewModel>(okResult.Value);
            Assert.Equal(color.Id, deletedColor.Id);
        }

        [Fact]
        public async Task DeleteColor_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteColor(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetColorsPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetColorsPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<ColorViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
