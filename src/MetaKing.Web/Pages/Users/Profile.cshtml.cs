using MetaKing.System.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MetaKing.Pages.Users
{
    public class ProfileModel : PageModel
    {
        private readonly IUsersAppService _usersAppService;

        public ProfileModel(IUsersAppService usersAppService)
        {
            _usersAppService = usersAppService;
        }

        // Model để hiển thị ra View
        public UserDto UserInfo { get; set; }

        public async Task OnGetAsync()
        {
            // Gọi hàm lấy user hiện tại
            UserInfo = await _usersAppService.GetCurrentUserAsync();
        }
    }
}
