using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace MetaKing.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public RegisterModel(IConfiguration configuraiton)
        {
            _configuration = configuraiton;
        }
        public IActionResult OnGet()
        {
            return Redirect($"{_configuration["AuthServer:Authority"]}/Account/Register?returnUrl=https://localhost:5002/signin-oidc");
        }
    }
}
