using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Reflection;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Yunity.Controllers
{
    public class UGetPackController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;
        public UGetPackController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> List (string typeFilter, int page = 1)
        {
            BuildingDataContext db = new BuildingDataContext();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound("無法找到用戶對應的大樓資訊。");
            }

            int userId=(int)userInfo.FId;
            int userBdId = (int)userInfo.FBuildingId;

            
            var doorNos = db.BdMembers
                .Where(b => b.BdId == userBdId && b.UserId==userId)
                .Select(b => new { b.DoorNo, b.BdId })
                .ToList();

            var allPacks = db.GetPacks.ToList();

            var datas = allPacks.Where(p =>
                doorNos.Any(d =>
                    d.DoorNo == db.DoorNos
                        .Where(dn => dn.Id == p.DoorNoId)
                        .Select(dn => dn.DoorNo1)
                        .FirstOrDefault() &&
                    d.BdId == p.BdId)  
            ).ToList();

            
            if (!string.IsNullOrEmpty(typeFilter))
            {
                datas = datas.Where(d => GetPackageType(d.Type).Contains(typeFilter)).ToList();
            }

            
            datas = datas.OrderBy(d => d.State != 0)
                         .ThenBy(d => d.LogTime)
                         .ToList();

            bool hasUncollectedPackage = datas.Any(d => d.State == 0);

            var result = datas.Select(d => new GetPacksWrap
            {
                Id = d.Id,
                PackNo = d.PackNo,
                Type = GetPackageType(d.Type),  
                DoorNoId = d.DoorNoId,
                ManagerId = d.ManagerId,
                GetUser = d.GetUser,
                Logistic = d.Logistic,
                LogTime = d.LogTime,
                PackPhoto = "/GetPackPhoto/" + d.PackPhoto,
                PickUser = d.PickUser,
                PickTime = d.PickTime,
                State = GetState(d.State)
            }).ToList();

            ViewBag.hasUncollectedPackage = hasUncollectedPackage;

            return View(result);

        }
        public string GetPackageType(int? type)
        {
            if (type == null)
            {
                return "未知類型";
            }

            switch (type)
            {
                case 1:
                    return "信件";
                case 2:
                    return "一般";
                case 3:
                    return "冷藏";
                default:
                    return "其他";
            }
        }
        public string GetState(int? state)
        {


            if (state == null)
            {
                return "未知狀態";  
            }

            switch (state)
            {
                case 0:
                    return "未領取";
                case 1:
                    return "已領取";
                default:
                    return "其他狀態";  
            }
        }

        public async Task<IActionResult> FilterAjax(int state)
        {
            BuildingDataContext db = new BuildingDataContext();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound("無法找到用戶對應的大樓資訊。");
            }
            int userId = (int)userInfo.FId;
            int userBdId = (int)userInfo.FBuildingId;

            // 根據用戶的 ID 查找該用戶的門牌號及其所在大樓
            var doorNos = db.BdMembers
                .Where(b => b.BdId == userBdId && b.UserId == userId)
                .Select(b => new { b.DoorNo, b.BdId })
                .ToList();

            var allPacks = db.GetPacks.ToList();

            var datas = allPacks.Where(p =>
                doorNos.Any(d =>
                    d.DoorNo == db.DoorNos
                        .Where(dn => dn.Id == p.DoorNoId)
                        .Select(dn => dn.DoorNo1)
                        .FirstOrDefault() &&
                    d.BdId == p.BdId) // BdId 匹配
            ).ToList();

            // 根據state進行篩選
            if (state != -1)
            {
                datas = datas.Where(d => d.State == state).ToList();
            }

            // 根據狀態排序，未領取排到最前
            datas = datas.OrderBy(d => d.State != 0)
                         .ThenBy(d => d.LogTime)
                         .ToList();

            var result = datas.Select(d => new GetPacksWrap
            {
                Id = d.Id,
                PackNo = d.PackNo,
                Type = GetPackageType(d.Type),  // 將包裹類型轉換為中文
                DoorNoId = d.DoorNoId,
                ManagerId = d.ManagerId,
                GetUser = d.GetUser,
                Logistic = d.Logistic,
                LogTime = d.LogTime,
                PackPhoto = "/GetPackPhoto/" + d.PackPhoto,
                PickUser = d.PickUser,
                PickTime = d.PickTime,
                State = GetState(d.State)
            }).ToList();

            // 返回部分視圖的結果
            return PartialView("_GetPackList", result); // 返回篩選後的資料
        }


    }
}
