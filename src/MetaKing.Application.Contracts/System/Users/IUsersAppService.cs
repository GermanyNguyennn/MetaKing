using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MetaKing.System.Users
{
    public interface IUsersAppService : ICrudAppService<UserDto, Guid, PagedResultRequestDto>
    {
        Task SetPasswordAsync(Guid userId, SetPasswordDto input);
        Task<UserDto> GetCurrentUserAsync();
    }
}
