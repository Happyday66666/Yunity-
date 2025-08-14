using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class ConciergeController : Controller
    {
        private readonly BuildingDataContext _BDcontext;
        private readonly UserManager<YunityUser> _userManager;

        public ConciergeController(BuildingDataContext BDcontext, UserManager<YunityUser> userManager)
        {
            _BDcontext = BDcontext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Role !="Manager")
            {
                return Challenge(); // 未登入時，要求登入
            }

            // 查詢管理員資料，若不存在則不允許存取
            var managerInfo = await _BDcontext.TManagerInfos.FirstOrDefaultAsync(m => m.FAspUserId == user.Id);
            if (managerInfo == null)
            {
                return Challenge(); // 若 TManagerInfo 為 null，則要求登入或拒絕存取
            }

            return View();
        }

        //布告欄
        //GET:Concierge/GetBoardList
        [HttpGet]
        public JsonResult GetBoardList()
        {
            var boards = _BDcontext.Boards
                .Where(b => b.State == 3)
                .OrderByDescending(b => b.OpenTime)
                .Take(7) 
                .Select(b => new
                {
                    b.Id,
                    b.Type,
                    b.Name,
                    b.State,
                    Time = b.OpenTime
                })
                .ToList();

            return Json(boards);
        }


    }
}
