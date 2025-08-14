using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using Yunity.Areas.Identity.Data;
using Yunity.Data;
using Yunity.Models;


namespace Yunity.Controllers
{
    public class RECalendarController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public RECalendarController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // 未登入時，要求登入
            }

            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound("無法找到用戶對應的大樓資訊。");
            }

            int userBdId = (int)userInfo.FBuildingId;

            return View();
        }
        public async Task<IActionResult> GetEvents(DateTime? start, DateTime? end)
        {

            // 設定默認的月份範圍
            DateTime monthStart = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime monthEnd = end ?? monthStart.AddMonths(1).AddDays(-1);

             
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

            var events = new List<object>();
            try
            {
                using (var db = new BuildingDataContext())
                {
                    // 查詢該用戶的預約資料，並加入篩選條件（時間範圍和用戶ID）
                    var reservations = db.UserAreaReserves
                        .Where(r => r.UserId == userBdId && r.StartTime >= monthStart && r.EndTime <= monthEnd)
                        .Join(db.PublicAreaReserves,  // 聯接 PublicAreaReserves 表
                              reserve => reserve.AreaId,
                              area => area.Id,
                              (reserve, area) => new
                              {
                                  reserve.Id,
                                  area.Name,  // 區域名稱
                                  reserve.StartTime,
                                  reserve.EndTime,
                              })
                        .ToList();  // 儘量減少查詢結果

                    foreach (var reservation in reservations)
                    {
                        var eventObj = new
                        {
                            title = reservation.Name,  // 顯示區域名稱作為標題
                            start = reservation.StartTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "",  // 開始時間
                            end = reservation.EndTime?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "",  // 結束時間
                            color = "#FF5733",  // 設定顏色
                        };
                        events.Add(eventObj);  // 加入事件資料
                    }
                }
            }
            catch (Exception ex)
            {
                // 日誌記錄錯誤，或顯示錯誤訊息
                // 可以寫錯誤日誌，或者返回錯誤訊息給前端
                Console.Error.WriteLine($"Error occurred while fetching events: {ex.Message}");
            }

            return Json(events);  // 返回事件資料的JSON
        }
    }
}
