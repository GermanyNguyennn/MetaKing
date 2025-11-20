using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace MetaKing.System.Users
{
    public class UsersAppService : CrudAppService<IdentityUser, UserDto, Guid, PagedResultRequestDto>, IUsersAppService
    {
        private readonly IdentityUserManager _identityUserManager;

        public UsersAppService(IRepository<IdentityUser, Guid> repository,
            IdentityUserManager identityUserManager) : base(repository)
        {
            _identityUserManager = identityUserManager;
        }
       
        public async Task SetPasswordAsync(Guid userId, SetPasswordDto input)
        {
            var user = await _identityUserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(IdentityUser), userId);
            }
            var token = await _identityUserManager.GeneratePasswordResetTokenAsync(user);
            var result = await _identityUserManager.ResetPasswordAsync(user, token, input.NewPassword);
            if (!result.Succeeded)
            {
                List<Microsoft.AspNetCore.Identity.IdentityError> errorList = result.Errors.ToList();
                string errors = "";

                foreach (var error in errorList)
                {
                    errors = errors + error.Description.ToString();
                }
                throw new UserFriendlyException(errors);
            }
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                throw new UserFriendlyException("Bạn chưa đăng nhập!");
            }

            // Lấy user từ DB
            var user = await _identityUserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(IdentityUser), userId);
            }

            // Lấy danh sách roles
            var roles = await _identityUserManager.GetRolesAsync(user);

            // Map sang UserDto
            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roles // IList<string>
            };

            return dto;
        }
    }
}
