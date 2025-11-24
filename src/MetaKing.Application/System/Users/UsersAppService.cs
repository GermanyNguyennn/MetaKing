using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UsersAppService> _logger;

        public UsersAppService(IRepository<IdentityUser, Guid> repository,
            IdentityUserManager identityUserManager,
            ILogger<UsersAppService> logger) : base(repository)
        {
            _identityUserManager = identityUserManager;
            _logger = logger;
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
                throw new UserFriendlyException(string.Join("; ", result.Errors.Select(e => e.Description)));

                //List<Microsoft.AspNetCore.Identity.IdentityError> errorList = result.Errors.ToList();
                //string errors = "";

                //foreach (var error in errorList)
                //{
                //    errors = errors + error.Description.ToString();
                //}
                //throw new UserFriendlyException(errors);
            }
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            _logger.LogInformation("GetCurrentUserAsync called. IsAuthenticated={IsAuthenticated}, UserId={UserId}", CurrentUser.IsAuthenticated, CurrentUser.Id);

            try
            {
                var httpContextAccessor = LazyServiceProvider.LazyGetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
                var httpUser = httpContextAccessor?.HttpContext?.User;
                if (httpUser != null)
                {
                    foreach (var claim in httpUser.Claims)
                    {
                        try
                        {
                            _logger.LogInformation("HttpContext User Claim: {Type} = {Value}", claim.Type, claim.Value);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enumerate HttpContext.User claims");
            }

            if (!CurrentUser.IsAuthenticated || CurrentUser.Id == null)
            {
                _logger.LogWarning("GetCurrentUserAsync: current user is not authenticated or missing Id.");
                throw new UserFriendlyException("Bạn chưa đăng nhập!");
            }

            var user = await _identityUserManager.FindByIdAsync(CurrentUser.Id.ToString())
                       ?? throw new EntityNotFoundException(typeof(IdentityUser), CurrentUser.Id);

            var dto = ObjectMapper.Map<IdentityUser, UserDto>(user);
            dto.Roles = (await _identityUserManager.GetRolesAsync(user)).ToList();

            var logger = LazyServiceProvider.LazyGetRequiredService<ILogger<UsersAppService>>();
            logger.LogInformation("GetCurrentUserAsync called. IsAuthenticated={IsAuthenticated}, UserId={UserId}", CurrentUser.IsAuthenticated, CurrentUser.Id);

            return dto;
        }
    }
}
