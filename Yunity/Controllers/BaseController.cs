using Microsoft.AspNetCore.Mvc;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

            ViewBag.user_name = CHome_Model.user_name;

        }
    }
}
