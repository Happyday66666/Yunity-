using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class PublicAreaReservesController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public PublicAreaReservesController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
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


            // 取得該大樓所有的公設資料
            var publicAreas = await _context.PublicAreaReserves
                .Where(p => p.BdId == userBdId)
                .ToListAsync();

            var viewModelList = new List<PublicAreaReserveViewModel>();

            // 依次處理每個公設，計算可預約名額並建立視圖模型
            foreach (var area in publicAreas)
            {
                // 計算可預約名額
                int availableSpots = await GetAvailableSpots(area.Id, DateTime.Now); // 假設現在的時間為預約時間

                // 添加到視圖模型列表中
                viewModelList.Add(new PublicAreaReserveViewModel
                {
                    Id = area.Id,
                    Name = area.Name,
                    OpenTime = area.OpenTime,
                    CloseTime = area.CloseTime,
                    DeductUnit = area.DeductUnit,
                    //UseTime = area.UseTime,
                    UseTime = area.UseTimeUnit?.ToString() ?? "不限時",

                    AreaInfo = area.AreaInfo,
                    Amount = area.Amount,
                    
                    AvailableSpots = availableSpots,
                    //Photo = area.Photo // 這裡確保 photo 會被讀取
                     Photo = string.IsNullOrEmpty(area.Photo)
            ? Url.Content("~/publicArea/default-placeholder.jpg")
            : Url.Content("~/publicArea/" + area.Photo)
                });
            }

            // 返回視圖並傳遞公設資料
            return View(viewModelList);
        }



        // GET: PublicAreaReserves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicAreaReserve = await _context.PublicAreaReserves
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publicAreaReserve == null)
            {
                return NotFound();
            }

            return View(publicAreaReserve);
        }

        // GET: PublicAreaReserves/Create
        public IActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BdId,Name,OpenTime,CloseTime,DeductUnit,UseTime,AreaInfo,Amount")] PublicAreaReserve publicAreaReserve)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publicAreaReserve);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publicAreaReserve);
        }

        // GET: PublicAreaReserves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicAreaReserve = await _context.PublicAreaReserves.FindAsync(id);
            if (publicAreaReserve == null)
            {
                return NotFound();
            }
            return View(publicAreaReserve);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BdId,Name,OpenTime,CloseTime,DeductUnit,UseTime,AreaInfo,Amount")] PublicAreaReserve publicAreaReserve)
        {
            if (id != publicAreaReserve.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publicAreaReserve);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicAreaReserveExists(publicAreaReserve.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(publicAreaReserve);
        }

        // GET: PublicAreaReserves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicAreaReserve = await _context.PublicAreaReserves
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publicAreaReserve == null)
            {
                return NotFound();
            }

            return View(publicAreaReserve);
        }

        // POST: PublicAreaReserves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publicAreaReserve = await _context.PublicAreaReserves.FindAsync(id);
            if (publicAreaReserve != null)
            {
                _context.PublicAreaReserves.Remove(publicAreaReserve);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicAreaReserveExists(int id)
        {
            return _context.PublicAreaReserves.Any(e => e.Id == id);
        }

        private async Task<int> GetAvailableSpots(int areaId, DateTime reserveTime)
        {
            var publicArea = await _context.PublicAreaReserves
                .FirstOrDefaultAsync(p => p.Id == areaId);

            if (publicArea == null)
                return 0; // 若找不到公設，回傳 0

            // 計算該預約日期的開始和結束時間，將 TimeOnly 轉換為 DateTime
            var reserveStartTime = reserveTime.Date.Add(new TimeSpan(publicArea.OpenTime?.Hour ?? 0, publicArea.OpenTime?.Minute ?? 0, publicArea.OpenTime?.Second ?? 0));
            var reserveEndTime = reserveTime.Date.Add(new TimeSpan(publicArea.CloseTime?.Hour ?? 0, publicArea.CloseTime?.Minute ?? 0, publicArea.CloseTime?.Second ?? 0));

            // 篩選出已經預約的時間記錄
            var reservedCount = await _context.UserAreaReserves
                .Where(r => r.AreaId == areaId &&
                            r.StartTime >= reserveStartTime &&
                            r.EndTime < reserveEndTime &&
                            r.State == 1)
                .SumAsync(r => r.ReservedPeople); // 計算每筆預約的人數（假設 PeopleCount 是人數欄位）// 只計算已預約的

            int availableSpots = (int)(publicArea.Amount.GetValueOrDefault() - reservedCount);

            return availableSpots > 0 ? availableSpots : 0; // 確保不回傳負數
            
        }






    }
}
