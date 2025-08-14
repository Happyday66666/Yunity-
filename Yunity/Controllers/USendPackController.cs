using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Controllers
{
    public class USendPackController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;
        public USendPackController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> List(string typeFilter, int page = 1)
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

           

            var datas = db.SendPacks
                .Where(sp => sp.UserId == userInfo.FId)  
                .Join(db.TusersInfos, sp => sp.UserId, user => user.FId, (sp, user) => new { sp, user })
                .Select(d => new SendPacksWrap
                {
                    Id = d.sp.Id,
                    UserName = d.user.FName,  
                    Type = SendPackageType(d.sp.Type),  
                    GetUser = d.sp.GetUser,
                    GetTel = d.sp.GetTel,
                    SendAddr = d.sp.SendAddr,
                    Logistic = d.sp.Logistic,
                    PickUser = d.sp.PickUser,
                    PickTime = d.sp.PickTime,
                    PackPhoto = "/SendPackPicture/" + d.sp.PackPhoto,
                    PickLogisticTime = d.sp.PickLogisticTime,
                    State = SendState(d.sp.State)  
                })
                .ToList();

            
            if (!string.IsNullOrEmpty(typeFilter))
            {
                datas = datas.Where(d => d.Type.Contains(typeFilter)).ToList();
            }

            datas = datas.OrderBy(d => d.State == "待處理" ? 0 : 1)   
                 .ThenBy(d => d.UserName)  
                 .ToList();

             

            return View(datas);
        }


        public static string SendPackageType(int? type)
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
                    return "掛號";
                case 3:
                    return "一般包裹";
                case 4:
                    return "冷藏包裹";
                case 5:
                    return "冷凍包裹";
                default:
                    return "其他";
            }
        }

        public static string SendState(int? state)
        {
            if (state == null)
            {
                return "未知狀態";
            }

            switch (state)
            {
                case 0:
                    return "待處理";
                case 1:
                    return "已寄送";
                default:
                    return "其他狀態";
            }
        }

        //Get:USendPack/FilterAjax
        [HttpGet]
        public IActionResult FilterAjax(int state)
        {
            BuildingDataContext db = new BuildingDataContext();
            IQueryable<SendPacksWrap> filteredPackages;

            if (state == -1)   
            {
                
                filteredPackages = db.SendPacks
                    .Join(db.TusersInfos, sp => sp.UserId, user => user.FId, (sp, user) => new { sp, user })
                    .Select(d => new SendPacksWrap
                    {
                        Id = d.sp.Id,
                        UserName = d.user.FName,
                        Type = SendPackageType(d.sp.Type),
                        GetUser = d.sp.GetUser,
                        GetTel = d.sp.GetTel,
                        SendAddr = d.sp.SendAddr,
                        Logistic = d.sp.Logistic,
                        PickUser = d.sp.PickUser,
                        PickTime = d.sp.PickTime,
                        PackPhoto = "/SendPackPicture/" + d.sp.PackPhoto,
                        PickLogisticTime = d.sp.PickLogisticTime,
                        State = SendState(d.sp.State)  
                    });
            }
            else
            {
                // 已寄出或未寄出---篩選
                filteredPackages = db.SendPacks
                    .Where(p => p.State == state)  
                    .Join(db.TusersInfos, sp => sp.UserId, user => user.FId, (sp, user) => new { sp, user })
                    .Select(d => new SendPacksWrap
                    {
                        Id = d.sp.Id,
                        UserName = d.user.FName,
                        Type = SendPackageType(d.sp.Type),
                        GetUser = d.sp.GetUser,
                        GetTel = d.sp.GetTel,
                        SendAddr = d.sp.SendAddr,
                        Logistic = d.sp.Logistic,
                        PickUser = d.sp.PickUser,
                        PickTime = d.sp.PickTime,
                        PackPhoto = "/SendPackPicture/" + d.sp.PackPhoto,
                        PickLogisticTime = d.sp.PickLogisticTime,
                        State = SendState(d.sp.State)  
                    });
            }

            return PartialView("_SendPacksList", filteredPackages.ToList());
        }

        public IActionResult Details(int? Id)
        {
            BuildingDataContext db = new BuildingDataContext();
            IQueryable<SendPacksWrap> filteredPackages;

            var sendPack = db.SendPacks
                .Where(sp => sp.Id == Id.Value)
                .Join(db.TusersInfos, sp => sp.UserId, user => user.FId, (sp, user) => new { sp, user })
                .Select(d => new SendPacksWrap
                {
                    Id = d.sp.Id,
                    UserName = d.user.FName,
                    Type = SendPackageType(d.sp.Type),
                    GetUser = d.sp.GetUser,
                    GetTel = d.sp.GetTel,
                    SendAddr = d.sp.SendAddr,
                    Logistic = d.sp.Logistic,
                    PickUser = d.sp.PickUser,
                    PickTime = d.sp.PickTime,
                    PackPhoto = "/SendPackPicture/" + d.sp.PackPhoto,
                    PickLogisticTime = d.sp.PickLogisticTime,
                    State = SendState(d.sp.State)

                }).FirstOrDefault();
            return View(sendPack);
        }
    }
}
