using Microsoft.AspNetCore.Mvc;

namespace MetaKing.BackendServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
