using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Controllers
{
    public class UBoardController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;
        public UBoardController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> List()
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

            int userBdId = (int)userInfo.FBuildingId;


            var query = from t in _context.Boards
                        join bd in _context.BdLists on t.BdId equals bd.Id into bdGroup
                        from bd in bdGroup.DefaultIfEmpty()
                        where t.BdId == userBdId && t.State == 3
                        select new Yunity.Models.CBoardwarp
                        {
                            Id = t.Id,
                            BdId = t.BdId,
                            BdName = bd.BdName,
                            Name = t.Name,
                            Info = t.Info,
                            Photo = "/boardPhoto/" + t.Photo,
                            OpenTime = t.OpenTime,
                            State = t.State,
                            Type = t.Type
                        };

             

            var filteredData = query.ToList();
            return View(filteredData);
        }






        //Get:USendPack/FilterAjax
        [HttpGet]
        public IActionResult FilterAjax(int state)
        {
            BuildingDataContext db = new BuildingDataContext();

            // 根據state過濾
            IQueryable<Yunity.Models.CBoardwarp> filteredBoards;

            if (state == -1)
            {
                // 如果是-1，代表不篩選，顯示全部
                filteredBoards = from t in db.Boards
                                 join bd in db.BdLists on t.BdId equals bd.Id into bdGroup
                                 from bd in bdGroup.DefaultIfEmpty()
                                 where t.State == 3 // 你可能需要根據不同的需求來過濾其他條件
                                 select new Yunity.Models.CBoardwarp
                                 {
                                     Id = t.Id,
                                     BdId = t.BdId,
                                     BdName = bd.BdName,
                                     Name = t.Name,
                                     Info = t.Info,
                                     Photo = "/boardPhoto/" + t.Photo,
                                     OpenTime = t.OpenTime,
                                     State = t.State,
                                     Type = t.Type
                                 };
            }
            else
            {
                // 根據state來過濾
                filteredBoards = from t in db.Boards
                                 join bd in db.BdLists on t.BdId equals bd.Id into bdGroup
                                 from bd in bdGroup.DefaultIfEmpty()
                                 where t.State == 3 && t.Type == state // 根據Type來過濾
                                 select new Yunity.Models.CBoardwarp
                                 {
                                     Id = t.Id,
                                     BdId = t.BdId,
                                     BdName = bd.BdName,
                                     Name = t.Name,
                                     Info = t.Info,
                                     Photo = "/boardPhoto/" + t.Photo,
                                     OpenTime = t.OpenTime,
                                     State = t.State,
                                     Type = t.Type
                                 };
            }

            // 執行查詢並轉換為列表
            var result = filteredBoards.ToList();

            // 返回部分視圖，這裡假設部分視圖是 "_BoardList"
            return PartialView("_BoardList", result);
        }
        public IActionResult Details(int? id)
        {
            BuildingDataContext db = new BuildingDataContext();
            //分頁

            //找資料
            var board = db.Boards
                .Where(t => t.Id == id.Value)
                .Join(db.BdLists, t => t.BdId, bd => bd.Id, (t, bd) => new
                {
                    t.Id,
                    t.BdId,
                    BdName = bd.BdName,
                    t.Name,
                    t.Info,
                    t.Photo,
                    t.OpenTime,
                    t.State,
                    t.Type
                }).FirstOrDefault();

            if (board == null)
            {
                return NotFound();  // 如果找不到資料，返回404頁面
            }
            var boardDetails = new Yunity.Models.CBoardwarp
            {
                Id = board.Id,
                BdId = board.BdId,
                BdName = board.BdName,
                Name = board.Name,
                Info = board.Info,
                Photo = "/boardPhoto/" + board.Photo,
                OpenTime = board.OpenTime,
                State = board.State,
                Type = board.Type
            };
            return View(boardDetails);



        }
 

    }
}
