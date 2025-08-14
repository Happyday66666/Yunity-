using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using WebPWrecover.Services;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class UuserFeedbackController : Controller
    {
        IEmailSender _emailSender;
        private readonly UserManager<YunityUser> _userManager;
        private readonly BuildingDataContext _context;

        public UuserFeedbackController(IEmailSender emailSender, BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            return View();
        }

        [HttpPost]
        public JsonResult Sendmail ([FromBody] CKeywordViewModel key)
        {
            
            string info = "";
            _emailSender.SendEmailAsync("ineedyunity@gmail.com", key.title, key.body);
            return Json(1);
        }
    }
}
