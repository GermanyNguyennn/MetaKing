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
    public class VersionsControllerTest
    {
        private readonly VersionsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public VersionsControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Versions.Add(new VersionModel
            {
                Name = "Test Version",
                Description = "Description",
                Slug = "test-version",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new VersionsController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetVersions_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<VersionViewModel>>("Versions"))
                .ReturnsAsync((List<VersionViewModel>)null);

            // Act
            var result = await _controller.GetVersions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var versions = Assert.IsAssignableFrom<List<VersionViewModel>>(okResult.Value);
            Assert.NotEmpty(versions);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsVersion()
        {
            // Arrange
            var existing = _context.Versions.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var version = Assert.IsType<VersionViewModel>(okResult.Value);
            Assert.Equal(existing.Id, version.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostVersion_ValidRequest_ShouldCreateVersion()
        {
            var request = new VersionCreateRequest
            {
                Name = "New Version",
                Description = "New Description",
                Slug = "new-version",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostVersion(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutVersion_ExistingId_UpdatesVersion()
        {
            var version = _context.Versions.First();
            var request = new VersionCreateRequest
            {
                Name = "Updated",
                Description = "Updated Description",
                Slug = "updated-slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutVersion(version.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutVersion_NotFound_ReturnsNotFound()
        {
            var request = new VersionCreateRequest
            {
                Name = "Name",
                Description = "Desc",
                Slug = "slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutVersion(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteVersion_ExistingId_DeletesVersion()
        {
            var version = _context.Versions.First();
            var result = await _controller.DeleteVersion(version.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedVersion = Assert.IsType<VersionViewModel>(okResult.Value);
            Assert.Equal(version.Id, deletedVersion.Id);
        }

        [Fact]
        public async Task DeleteVersion_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteVersion(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetVersionsPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetVersionsPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<VersionViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
