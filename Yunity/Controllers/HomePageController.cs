using Microsoft.AspNetCore.Mvc;

namespace Yunity.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
