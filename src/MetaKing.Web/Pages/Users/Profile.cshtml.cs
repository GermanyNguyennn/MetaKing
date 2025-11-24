using MetaKing.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MetaKing.Pages.Users
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUsersAppService _usersAppService;

        public ProfileModel(IUsersAppService usersAppService)
        {
            _usersAppService = usersAppService;
        }

        public UserDto UserInfo { get; set; }

        public async Task OnGetAsync()
        {
            UserInfo = await _usersAppService.GetCurrentUserAsync();
        }
    }
}
