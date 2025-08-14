using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class AjaxController : Controller
    {
        BuildingDataContext _context;

        public AjaxController(BuildingDataContext context)
        {
            _context = context;
        }
        // Post: Ajax/UpdatePermission
        [HttpPost]
        public async Task <IActionResult> UpdatePermission(int id, [FromBody] BdPermission model)
        {

            if (model == null)
            {
                return Json(new { success = false, message = "无效的数据" });
            }

            var BdPermission = await _context.BdPermissions.FirstOrDefaultAsync(p => p.Id == id);

            if (BdPermission == null)
            {
                return Json(new { success = false, message = "找不到相关权限记录" });
            }
            BdPermission.ReceivePackage = model.ReceivePackage;
            BdPermission.SentPackage = model.SentPackage;
            BdPermission.ManageFee = model.ManageFee;
            BdPermission.Bulletin = model.Bulletin;
            BdPermission.PublicAreaReserve = model.PublicAreaReserve;
            BdPermission.NearbyStore = model.NearbyStore;
            BdPermission.CommunityService = model.CommunityService;
            BdPermission.Feedback = model.Feedback;
            BdPermission.VisitorRecord = model.VisitorRecord;
            BdPermission.Calendar = model.Calendar;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}
