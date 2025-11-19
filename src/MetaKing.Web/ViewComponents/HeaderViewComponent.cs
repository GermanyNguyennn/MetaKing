using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MetaKing.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
