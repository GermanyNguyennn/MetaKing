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
    public class CompaniesControllerTest
    {
        private readonly CompaniesController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public CompaniesControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Companies.Add(new CompanyModel
            {
                Name = "Test Company",
                Description = "Description",
                Slug = "test-company",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new CompaniesController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetCompanies_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<CompanyViewModel>>("Companies"))
                .ReturnsAsync((List<CompanyViewModel>)null);

            // Act
            var result = await _controller.GetCompanies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var companies = Assert.IsAssignableFrom<List<CompanyViewModel>>(okResult.Value);
            Assert.NotEmpty(companies);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsCompany()
        {
            // Arrange
            var existing = _context.Companies.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var company = Assert.IsType<CompanyViewModel>(okResult.Value);
            Assert.Equal(existing.Id, company.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostCompany_ValidRequest_ShouldCreateCompany()
        {
            var request = new CompanyCreateRequest
            {
                Name = "New Company",
                Description = "New Description",
                Slug = "new-company",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostCompany(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutCompany_ExistingId_UpdatesCompany()
        {
            var company = _context.Companies.First();
            var request = new CompanyCreateRequest
            {
                Name = "Updated",
                Description = "Updated Description",
                Slug = "updated-slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutCompany(company.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutCompany_NotFound_ReturnsNotFound()
        {
            var request = new CompanyCreateRequest
            {
                Name = "Name",
                Description = "Desc",
                Slug = "slug",
                Status = StatusType.Active,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutCompany(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCompany_ExistingId_DeletesCompany()
        {
            var company = _context.Companies.First();
            var result = await _controller.DeleteCompany(company.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedCompany = Assert.IsType<CompanyViewModel>(okResult.Value);
            Assert.Equal(company.Id, deletedCompany.Id);
        }

        [Fact]
        public async Task DeleteCompany_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteCompany(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetCompaniesPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetCompaniesPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<CompanyViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
