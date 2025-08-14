using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class UVisitorRecordController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public UVisitorRecordController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetVisitorList(int pageNumber = 1, int pageSize = 10)
        {
         
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "用戶未找到" });
            }

            
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return Json(new { success = false, message = "無法找到用戶的大樓資訊" });
            }

             
            int userBdId = (int)userInfo.FBuildingId;

          
            var userDoor = await _context.BdMembers
                .Where(b => b.UserId == userInfo.FId && b.BdId == userBdId)
                .Select(b => b.DoorNoId)
                .FirstOrDefaultAsync();

            if (userDoor == null)
            {
                return Json(new { success = false, message = "未找到對應的門牌資料" });
            }

            var twoMonthsAgo = DateTime.Now.AddMonths(-2);
            var visitorRecords = _context.VisitorRecords
                .Join(_context.BdMembers,
                 p => new { p.BdId, p.DoorNoId },
                 b => new { BdId = b.BdId, DoorNoId = b.DoorNoId },
                 (p, b) => new { p, b })
                .Where(x => x.b.UserId == userInfo.FId && x.p.VisitTime >= twoMonthsAgo)
                .OrderBy(x => x.p.VisitTime);

            var pagedVisitorRecords = await visitorRecords
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                     x.p.Id,
                     x.p.VisitorName,
                     x.p.VisitReason,
                     x.p.VisitTime
                })
                .ToListAsync();
            var totalRecords = await visitorRecords.CountAsync();

            return Json(new
            {
                success = true,
                data = pagedVisitorRecords,
                totalRecords = totalRecords
            });
        }


         

        [HttpPost]
        public async Task<JsonResult> Create(VisitorRecord VR)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
                );

                return Json(new { success = false, message = errors });
            }

            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "用戶未找到" });
            }

             
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return Json(new { success = false, message = "無法找到用戶的大樓資訊" });
            }

            int userBdId = (int)userInfo.FBuildingId;
 
            var userDoor = await _context.BdMembers
                .Where(b => b.UserId == userInfo.FId && b.BdId == userBdId)
                .Select(b => b.DoorNoId)
                .FirstOrDefaultAsync();

            if (userDoor == null)
            {
                
                return Json(new { success = false, message = "未找到對應的門牌資料" });
            }

            Console.WriteLine($"查詢到的門戶號：{userDoor}");
            VisitorRecord V = new VisitorRecord
            {
                DoorNoId = userDoor ?? throw new Exception("門戶號為 null，無法創建訪客記錄"),   
                VisitorName = VR.VisitorName,
                VisitReason = VR.VisitReason,
                VisitTime = VR.VisitTime,
                BdId = userBdId  
            };

            _context.VisitorRecords.Add(V);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "訪客資訊已成功提交！" });
        }


    }
}

