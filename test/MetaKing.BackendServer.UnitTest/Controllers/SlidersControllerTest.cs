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

namespace MetaKing.BackendServer.UnitTest.Controllers
{
    public class SlidersControllerTest
    {
        private readonly SlidersController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public SlidersControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Sliders.Add(new SliderModel
            {
                Name = "Test Slider",
                Image = "Test Image",
                Description = "Description",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new SlidersController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetSliders_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<SliderViewModel>>("Sliders"))
                .ReturnsAsync((List<SliderViewModel>)null);

            // Act
            var result = await _controller.GetSliders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var Sliders = Assert.IsAssignableFrom<List<SliderViewModel>>(okResult.Value);
            Assert.NotEmpty(Sliders);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsSlider()
        {
            // Arrange
            var existing = _context.Sliders.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var slider = Assert.IsType<SliderViewModel>(okResult.Value);
            Assert.Equal(existing.Id, slider.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostSlider_ValidRequest_ShouldCreateSlider()
        {
            var request = new SliderCreateRequest
            {
                Name = "New Slider",
                Image = "New Image",
                Description = "New Description",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostSlider(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutSlider_ExistingId_UpdatesSlider()
        {
            var slider = _context.Sliders.First();
            var request = new SliderCreateRequest
            {
                Name = "Updated",
                Image = "Update Image",
                Description = "Updated Description",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutSlider(slider.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutSlider_NotFound_ReturnsNotFound()
        {
            var request = new SliderCreateRequest
            {
                Name = "Name",
                Image = "Image",
                Description = "Desc",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutSlider(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSlider_ExistingId_DeletesSlider()
        {
            var slider = _context.Sliders.First();
            var result = await _controller.DeleteSlider(slider.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedSlider = Assert.IsType<SliderViewModel>(okResult.Value);
            Assert.Equal(slider.Id, deletedSlider.Id);
        }

        [Fact]
        public async Task DeleteSlider_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteSlider(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetSlidersPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetSlidersPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<SliderViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
