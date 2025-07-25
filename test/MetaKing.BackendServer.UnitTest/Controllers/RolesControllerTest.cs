﻿using MetaKing.BackendServer.Controllers;
using MetaKing.BackendServer.Data;
using MetaKing.ViewModels;
using MetaKing.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MockQueryable.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaKing.BackendServer.UnitTest.Controllers
{
    public class RolesControllerTest
    {
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private ApplicationDbContext _context;

        private List<IdentityRole> _roleSources = new List<IdentityRole>(){
                             new IdentityRole("test1"),
                             new IdentityRole("test2"),
                             new IdentityRole("test3"),
                             new IdentityRole("test4")
                        };

        public RolesControllerTest()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            Assert.NotNull(rolesController);
        }

        [Fact]
        public async Task PostRole_ValidInput_Success()
        {
            _mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.PostRole(new RoleCreateRequest()
            {
                Id = "test",
                Name = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task PostRole_ValidInput_Failed()
        {
            _mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.PostRole(new RoleCreateRequest()
            {
                Id = "test",
                Name = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetRoles_HasData_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.Roles)
                .Returns(_roleSources.AsQueryable().BuildMockDbSet().Object);
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.GetRoles();
            var okResult = result as OkObjectResult;
            var RoleViewModels = okResult.Value as IEnumerable<RoleViewModel>;
            Assert.True(RoleViewModels.Count() > 0);
        }

        [Fact]
        public async Task GetRoles_ThrowException_Failed()
        {
            _mockRoleManager.Setup(x => x.Roles).Throws<Exception>();

            var rolesController = new RolesController(_mockRoleManager.Object, _context);

            await Assert.ThrowsAnyAsync<Exception>(async () => await rolesController.GetRoles());
        }

        [Fact]
        public async Task GetRolesPaging_NoFilter_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.Roles)
                .Returns(_roleSources.AsQueryable().BuildMockDbSet().Object);

            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.GetRolesPaging(null, 1, 2);
            var okResult = result as OkObjectResult;
            var RoleViewModels = okResult.Value as Pagination<RoleViewModel>;
            Assert.Equal(4, RoleViewModels.Total);
            Assert.Equal(2, RoleViewModels.Items.Count);
        }

        [Fact]
        public async Task GetRolesPaging_HasFilter_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.Roles)
                .Returns(_roleSources.AsQueryable().BuildMockDbSet().Object);

            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.GetRolesPaging("test3", 1, 2);
            var okResult = result as OkObjectResult;
            var RoleViewModels = okResult.Value as Pagination<RoleViewModel>;
            Assert.Equal(1, RoleViewModels.Total);
            Assert.Single(RoleViewModels.Items);
        }

        [Fact]
        public async Task GetRolesPaging_ThrowException_Failed()
        {
            _mockRoleManager.Setup(x => x.Roles).Throws<Exception>();

            var rolesController = new RolesController(_mockRoleManager.Object, _context);

            await Assert.ThrowsAnyAsync<Exception>(async () => await rolesController.GetRolesPaging(null, 1, 1));
        }

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole()
                {
                    Id = "test1",
                    Name = "test1"
                });
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.GetById("test1");
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var RoleViewModel = okResult.Value as RoleViewModel;

            Assert.Equal("test1", RoleViewModel.Name);
        }

        [Fact]
        public async Task GetById_ThrowException_Failed()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Throws<Exception>();

            var rolesController = new RolesController(_mockRoleManager.Object, _context);

            await Assert.ThrowsAnyAsync<Exception>(async () => await rolesController.GetById("test1"));
        }

        [Fact]
        public async Task PutRole_ValidInput_Success()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new IdentityRole()
               {
                   Id = "test",
                   Name = "test"
               });

            _mockRoleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.PutRole("test", new RoleCreateRequest()
            {
                Id = "test",
                Name = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutRole_ValidInput_Failed()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new IdentityRole()
             {
                 Id = "test",
                 Name = "test"
             });

            _mockRoleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.PutRole("test", new RoleCreateRequest()
            {
                Id = "test",
                Name = "test"
            });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ValidInput_Success()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new IdentityRole()
               {
                   Id = "test",
                   Name = "test"
               });

            _mockRoleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.DeleteRole("test");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_ValidInput_Failed()
        {
            _mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(new IdentityRole()
             {
                 Id = "test",
                 Name = "test"
             });

            _mockRoleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));

            var rolesController = new RolesController(_mockRoleManager.Object, _context);
            var result = await rolesController.DeleteRole("test");
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
