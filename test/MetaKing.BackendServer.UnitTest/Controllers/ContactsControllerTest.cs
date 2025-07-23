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
    public class ContactsControllerTest
    {
        private readonly ContactsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ICacheService> _mockCacheService;

        public ContactsControllerTest()
        {
            // Khởi tạo context sử dụng InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // mỗi test dùng DB mới
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed dữ liệu
            _context.Contacts.Add(new ContactModel
            {
                Name = "Test Contact",
                Description = "Description",
                Map = "Hà Nội",
                Email = "manhducnguyen23092003@gmail.com",
                Phone = "0964429403",
                Address = "Hà Nội",
                CreatedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            _mockCacheService = new Mock<ICacheService>();
            _controller = new ContactsController(_context, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetContacts_ShouldReturnOkResult()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<ContactViewModel>>("Contacts"))
                .ReturnsAsync((List<ContactViewModel>)null);

            // Act
            var result = await _controller.GetContacts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var Contacts = Assert.IsAssignableFrom<List<ContactViewModel>>(okResult.Value);
            Assert.NotEmpty(Contacts);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsContact()
        {
            // Arrange
            var existing = _context.Contacts.First();
            // Act
            var result = await _controller.GetById(existing.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var contact = Assert.IsType<ContactViewModel>(okResult.Value);
            Assert.Equal(existing.Id, contact.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostContact_ValidRequest_ShouldCreateContact()
        {
            var request = new ContactCreateRequest
            {
                Name = "New Contact",
                Description = "New Description",
                Map = "Hà Nội",
                Email = "manhducnguyen23092003@gmail.com",
                Phone = "0964429403",
                Address = "Hà Nội",
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PostContact(request);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
        }

        [Fact]
        public async Task PutContact_ExistingId_UpdatesContact()
        {
            var contact = _context.Contacts.First();
            var request = new ContactCreateRequest
            {
                Name = "Updated",
                Description = "Updated Description",
                Map = "Hà Nội",
                Email = "manhducnguyen23092003@gmail.com",
                Phone = "0964429403",
                Address = "Hà Nội",
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutContact(contact.Id, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutContact_NotFound_ReturnsNotFound()
        {
            var request = new ContactCreateRequest
            {
                Name = "Name",
                Description = "Desc",
                Map = "Hà Nội",
                Email = "manhducnguyen23092003@gmail.com",
                Phone = "0964429403",
                Address = "Hà Nội",
                CreatedDate = DateTime.UtcNow
            };

            var result = await _controller.PutContact(999, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteContact_ExistingId_DeletesContact()
        {
            var contact = _context.Contacts.First();
            var result = await _controller.DeleteContact(contact.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedContact = Assert.IsType<ContactViewModel>(okResult.Value);
            Assert.Equal(contact.Id, deletedContact.Id);
        }

        [Fact]
        public async Task DeleteContact_NotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteContact(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetContactsPaging_WithFilter_ReturnsFilteredItems()
        {
            var result = await _controller.GetContactsPaging("Test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = Assert.IsType<Pagination<ContactViewModel>>(okResult.Value);
            Assert.True(pagination.Total > 0);
        }
    }
}
