using MetaKing.BackendServer.Controllers;
using MetaKing.BackendServer.Data;
using MetaKing.ViewModels.Systems;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using MockQueryable;

namespace MetaKing.BackendServer.UnitTest.Controllers
{
    public class UsersControllerTest
    {
        private ApplicationDbContext _context;

        private readonly List<UserModel> _userSources = new List<UserModel>()
        {
            new UserModel("1", "test1", "test1@gmail.com", "0964429403", "Test 1", "23/09/2003", "Phố 8/3", "Hà Nội", "Hai Bà Trưng", "Quỳnh Mai", DateTime.UtcNow),
            new UserModel("2", "test2", "test2@gmail.com", "0964429403", "Test 2", "23/09/2003", "Phố 8/3", "Hà Nội", "Hai Bà Trưng", "Quỳnh Mai", DateTime.UtcNow),
            new UserModel("3", "test3", "test3@gmail.com", "0964429403", "Test 3", "23/09/2003", "Phố 8/3", "Hà Nội", "Hai Bà Trưng", "Quỳnh Mai", DateTime.UtcNow),
            new UserModel("4", "test4", "test4@gmail.com", "0964429403", "Test 4", "23/09/2003", "Phố 8/3", "Hà Nội", "Hai Bà Trưng", "Quỳnh Mai", DateTime.UtcNow),
        };

        public UsersControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
        }

        private Mock<UserManager<UserModel>> GetMockUserManager()
        {
            return new Mock<UserManager<UserModel>>(
                new Mock<IUserStore<UserModel>>().Object,
                null, null,
                new IUserValidator<UserModel>[0],
                new IPasswordValidator<UserModel>[0],
                null, null, null,
                new Mock<ILogger<UserManager<UserModel>>>().Object
            );
        }

        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            return new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                null, null, null
            );
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task PostUser_ValidInput_Success()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserModel { UserName = "test" });

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.PostUser(new UserCreateRequest { UserName = "test", Dob = "23/09/2003" });

            Assert.NotNull(result);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task PostUser_ValidInput_Failed()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.PostUser(new UserCreateRequest { UserName = "test", Dob = "23/09/2003" });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUsers_HasData_ReturnSuccess()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.Users)
                .Returns(_userSources.AsQueryable().BuildMockDbSet().Object);

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.GetUsers();

            var okResult = result as OkObjectResult;
            var users = okResult.Value as IEnumerable<UserViewModel>;

            Assert.True(users.Count() > 0);
        }

        [Fact]
        public async Task GetUsers_ThrowException_Failed()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.Users).Throws<Exception>();

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetUsers());
        }

        [Fact]
        public async Task GetUsersPaging_NoFilter_ReturnSuccess()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.Users)
                .Returns(_userSources.AsQueryable().BuildMockDbSet().Object);

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.GetUsersPaging(null, 1, 2);

            var okResult = result as OkObjectResult;
            var users = okResult.Value as Pagination<UserViewModel>;

            Assert.Equal(4, users.Total);
            Assert.Equal(2, users.Items.Count);
        }

        [Fact]
        public async Task GetUsersPaging_HasFilter_ReturnSuccess()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.Users)
                .Returns(_userSources.AsQueryable().BuildMockDbSet().Object);

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.GetUsersPaging("test3", 1, 2);

            var okResult = result as OkObjectResult;
            var users = okResult.Value as Pagination<UserViewModel>;

            Assert.Equal(1, users.Total);
            Assert.Single(users.Items);
        }

        [Fact]
        public async Task GetUsersPaging_ThrowException_Failed()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.Users).Throws<Exception>();

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetUsersPaging(null, 1, 1));
        }

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserModel { UserName = "test1" });

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.GetById("test1");

            var okResult = result as OkObjectResult;
            var user = okResult.Value as UserViewModel;

            Assert.Equal("test1", user.UserName);
        }

        [Fact]
        public async Task GetById_ThrowException_Failed()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .Throws<Exception>();

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            await Assert.ThrowsAnyAsync<Exception>(() => controller.GetById("test1"));
        }

        [Fact]
        public async Task PutUser_ValidInput_Success()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new UserModel { UserName = "test1" });

            userManager.Setup(x => x.UpdateAsync(It.IsAny<UserModel>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.PutUser("test", new UserCreateRequest { FullName = "test2", Dob = "23/09/2003" });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutUser_ValidInput_Failed()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserModel { UserName = "test1" });

            userManager.Setup(x => x.UpdateAsync(It.IsAny<UserModel>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.PutUser("test", new UserCreateRequest { UserName = "test1", Dob = "23/09/2003" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ValidInput_Success()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserModel { UserName = "test1" });

            userManager.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<UserModel> { new UserModel { UserName = "test1" } });

            userManager.Setup(x => x.DeleteAsync(It.IsAny<UserModel>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.DeleteUser("test");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ValidInput_Failed()
        {
            var userManager = GetMockUserManager();
            var roleManager = GetMockRoleManager();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserModel { UserName = "test1" });

            userManager.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<UserModel> { new UserModel { UserName = "test1" } });

            userManager.Setup(x => x.DeleteAsync(It.IsAny<UserModel>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var controller = new UsersController(userManager.Object, roleManager.Object, _context);
            var result = await controller.DeleteUser("test");

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
