using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    
    public class UserAreaReservesController : Controller
    {
       
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public UserAreaReservesController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
            
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAreaReserve = await _context.UserAreaReserves
                .Where(u => u.Id == id)
                .Select(u => new UserAreaReserveWithAreaInfo
                {
                    Id = u.Id,
                    AreaId = u.AreaId,
                    DoorNoId = u.DoorNoId,
                    UserId = u.UserId,
                    ReserveTime = u.ReserveTime,
                    StartTime = u.StartTime,
                    EndTime = u.EndTime,
                    State = u.State,
                    ReservedPeople= u.ReservedPeople,
                    PublicArea = _context.PublicAreaReserves.FirstOrDefault(p => p.Id == u.AreaId) // 取得公設資料
                })
                .FirstOrDefaultAsync();

            if (userAreaReserve == null)
            {
                return NotFound();
            }

            return View(userAreaReserve);
        }
              
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 取得預約資料 (UserAreaReserves)
            var reservationData = await _context.UserAreaReserves
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.AreaId,
                    u.DoorNoId,
                    u.UserId,
                    u.ReserveTime,
                    u.StartTime,
                    u.ReservedPeople
                })
                .FirstOrDefaultAsync();

            if (reservationData == null)
            {
                return NotFound();
            }

            // 取得對應公設資料 (Public_Area_Reserve)，以 AreaId 為關聯依據
            var publicArea = await _context.PublicAreaReserves
                .FirstOrDefaultAsync(p => p.Id == reservationData.AreaId);
            if (publicArea == null)
            {
                return NotFound();
            }

            // 取得公設的開放及結束時間 (取 TimeOfDay)
            TimeSpan openTime = publicArea.OpenTime.Value.ToTimeSpan();
            TimeSpan closeTime = publicArea.CloseTime.Value.ToTimeSpan();

            // 解析 UseTimeUnit (decimal) 為 TimeSpan (以小時為單位)
            TimeSpan slotDuration = TimeSpan.FromHours((double)publicArea.UseTimeUnit);

            // 計算預約日期：以預約資料的 StartTime.Date 為準
            DateTime reservationDate = reservationData.StartTime.Value.Date;
            DateTime dayStart = reservationDate + openTime;
            DateTime dayEnd = reservationDate + closeTime;

            // 建立時段列表，並在計算剩餘容量時排除自己的預約記錄
            var timeSlots = new List<TimeSlotViewModel>();
            DateTime current = dayStart;
            while (current < dayEnd)
            {
                DateTime slotStart = current;
                DateTime slotEnd = current + slotDuration;
                if (slotEnd > dayEnd)
                {
                    slotEnd = dayEnd;
                }
                // 時段字串，例如 "09:00~12:00"
                string slotText = $"{slotStart:HH:mm}~{slotEnd:HH:mm}";

                // 取得該時段中，除自己之外其他使用者已預約的人數
                int reservedCount = await _context.UserAreaReserves
                    .Where(u => u.AreaId == reservationData.AreaId
                             && u.ReserveTime.Value.Date == reservationDate
                             && u.StartTime >= slotStart
                             && u.StartTime < slotEnd
                             && u.Id != reservationData.Id)  // 排除自己的預約
                    .SumAsync(u => (int?)u.ReservedPeople) ?? 0;

                // 剩餘容量 = 公設的 Amount - 其他已預約人數
                int remainingCapacity = (int)publicArea.Amount - reservedCount;

                timeSlots.Add(new TimeSlotViewModel
                {
                    TimePeriod = slotText,
                    StartDateTime = slotStart,
                    EndDateTime = slotEnd,
                    RemainingCapacity = remainingCapacity
                });

                current = slotEnd;
            }

            // 建立下拉選單選項，Value 為時段字串，Text 為 "時段 (剩餘X人)"
            var timePeriodOptions = timeSlots.Select(ts => new SelectListItem
            {
                Value = ts.TimePeriod,
                Text = $"{ts.TimePeriod} (剩餘{ts.RemainingCapacity}人)"
            }).ToList();

            // 自動帶入原來的選擇
            string selectedTimePeriod = "";
            if (reservationData.StartTime.HasValue)
            {
                // 根據原始 StartTime，找出其所屬的時段
                foreach (var slot in timeSlots)
                {
                    if (reservationData.StartTime.Value >= slot.StartDateTime &&
                        reservationData.StartTime.Value < slot.EndDateTime)
                    {
                        selectedTimePeriod = slot.TimePeriod;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(selectedTimePeriod))
            {
                selectedTimePeriod = timePeriodOptions.FirstOrDefault()?.Value;
            }

            // 從 Door_No 資料表中取得實際使用者點數 (以 DoorNoId 對應)
            var doorNoInfo = await _context.DoorNos
                .FirstOrDefaultAsync(d => d.Id == reservationData.DoorNoId);
            if (doorNoInfo == null)
            {
                return NotFound("找不到對應的 Door_No 資料");
            }
            int userPoints = (int)doorNoInfo.Points;
            // 取得每人扣除點數
            int deductPoints = (int)publicArea.DeductUnit;
            // 原始預約人數（若為 null 則預設為 0）
            int originalReserved = reservationData.ReservedPeople ?? 0;
            // 進入編輯頁面時，新預約人數預設與原始預約人數相同
            int newReserved = originalReserved;
            // 根據公式計算總扣點數（初始為 0）與剩餘點數（初始則等於 userPoints）
            int totalDeductPoints = (newReserved - originalReserved) * deductPoints;
            int remainingPoints = userPoints - totalDeductPoints;

            // 建立 ViewModel
            var viewModel = new EditPublicAreaReservationViewModel
            {
                Id = reservationData.Id,
                AreaId = (int)reservationData.AreaId,
                DoorNoId = (int)reservationData.DoorNoId,
                UserId = (int)reservationData.UserId,
                ReserveTime = (DateTime)reservationData.ReserveTime,
                StartTime = (DateTime)reservationData.StartTime,
                Date = reservationDate,
                ReservedPeople = reservationData.ReservedPeople,
                // 帶入原來選擇的時段
                TimePeriod = selectedTimePeriod,
                TimePeriodOptions = timePeriodOptions,
                TimeSlots = timeSlots,
                PublicArea = new PublicAreaInfo
                {
                    Id = publicArea.Id,
                    Name = publicArea.Name,
                    Amount = publicArea.Amount,
                    DeductUnit = (int)publicArea.DeductUnit,
                    UseTimeUnit = (decimal)publicArea.UseTimeUnit,
                    Open_Time = publicArea.OpenTime,
                    Close_Time = publicArea.CloseTime
                },
                UserPoints = userPoints,
                DeductPoints = deductPoints,
                RemainingPoints = remainingPoints,
                TotalDeductPoints = totalDeductPoints
            };

            return View(viewModel);
        }

        // //建立新ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPublicAreaReservationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // 依需求重新組裝 viewModel（例如重新計算時段選項），此處略
                return View(viewModel);
            }

            // 取得原始預約記錄
            var reservation = await _context.UserAreaReserves.FindAsync(viewModel.Id);
            if (reservation == null)
            {
                return NotFound();
            }

            // 取得公設資料（以 AreaId 為關聯）
            var publicArea = await _context.PublicAreaReserves.FirstOrDefaultAsync(p => p.Id == reservation.AreaId);
            if (publicArea == null)
            {
                return NotFound();
            }

            // 取得 Door_No 資料，依據預約記錄中的 DoorNoId
            var doorNo = await _context.DoorNos.FindAsync(reservation.DoorNoId);
            if (doorNo == null)
            {
                return NotFound("找不到對應的 Door_No 資料");
            }

            // 每人扣除點數
            int deductPoints = (int)publicArea.DeductUnit;
            // 原始預約人數，若為 null 則預設為 0
            int originalReserved = reservation.ReservedPeople ?? 0;
            // 新預約人數來自 viewModel（預設至少為 1，由前端控管）
            int newReserved = viewModel.ReservedPeople ?? 1;
            // 差額 = 新 - 原
            int diff = newReserved - originalReserved;
            // 計算總扣點數
            int totalDeductPoints = diff * deductPoints;
            // 原始使用者點數（從 Door_No 取）
            int originalPoints = (int)doorNo.Points;
            int remainingPoints = originalPoints - totalDeductPoints;

            // 更新預約記錄的預約人數
            reservation.ReservedPeople = newReserved;

            // 處理時段：因 UserAreaReserve 沒有 TimePeriod 屬性，
            // 可解析 viewModel.TimePeriod（格式 "HH:mm~HH:mm"），以更新預約開始時間
            if (!string.IsNullOrEmpty(viewModel.TimePeriod))
            {
                // 解析出時段開始部分（例如 "09:00"）
                var parts = viewModel.TimePeriod.Split('~');
                if (parts.Length == 2)
                {
                    DateTime startTimePart;
                    // 嘗試以 "HH:mm" 格式解析
                    if (DateTime.TryParseExact(parts[0], "HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startTimePart))
                    {
                        // 預約日期以原來 ReserveTime 的日期為準
                        DateTime reservationDate = reservation.StartTime.Value.Date;
                        // 更新預約開始時間：預約日期 + 解析的時間
                        reservation.StartTime = reservationDate + startTimePart.TimeOfDay;
                    }
                }
            }
            // 新增：計算結束時間 = 開始時間 + UseTimeUnit 小時，並存回 EndTime
            reservation.EndTime = reservation.StartTime + TimeSpan.FromHours((double)publicArea.UseTimeUnit);
            // 更新 Door_No 中的點數：扣除總扣點數（若 diff 為負則退還點數）
            doorNo.Points = originalPoints - totalDeductPoints;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(viewModel.Id))
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

        private bool ReservationExists(int id)
        {
            return _context.UserAreaReserves.Any(e => e.Id == id);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAvailableSlots(int areaId, string date, string timeSlot)
        //{
        //    if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(timeSlot))
        //    {
        //        return BadRequest("日期與時段不可為空");
        //    }

        //    var area = await _context.PublicAreaReserves.FirstOrDefaultAsync(a => a.Id == areaId);
        //    if (area == null)
        //    {
        //        return NotFound("公設不存在");
        //    }

        //    int maxCapacity = area.Amount ?? 0;

        //    var timeParts = timeSlot.Split('~');
        //    if (timeParts.Length != 2)
        //    {
        //        return BadRequest("時段格式錯誤");
        //    }

        //    DateTime startTime = DateTime.Parse($"{date}T{timeParts[0]}:00");
        //    DateTime endTime = DateTime.Parse($"{date}T{timeParts[1]}:00");

        //    // 計算已預約人數
        //    int reservedPeople = await _context.UserAreaReserves
        //        .Where(r => r.AreaId == areaId && r.StartTime == startTime && r.EndTime == endTime)
        //        .SumAsync(r => r.ReservedPeople ?? 0);

        //    int availableSlots = maxCapacity - reservedPeople;
        //    return Ok(new { availableSlots });
        //}


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAreaReserve = await _context.UserAreaReserves
                .Where(u => u.Id == id)
                .Select(u => new UserAreaReserveWithAreaInfo
                {
                    Id = u.Id,
                    AreaId = u.AreaId,
                    DoorNoId = u.DoorNoId,
                    UserId = u.UserId,
                    ReserveTime = u.ReserveTime,
                    StartTime = u.StartTime,
                    EndTime = u.EndTime,
                    State = u.State,
                    PublicArea = _context.PublicAreaReserves.FirstOrDefault(p => p.Id == u.AreaId) // 取得公設名稱
                })
                .FirstOrDefaultAsync();

            if (userAreaReserve == null)
            {
                return NotFound();
            }

            return View(userAreaReserve);
        }


        // POST: UserAreaReserves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAreaReserve = await _context.UserAreaReserves.FindAsync(id);
            if (userAreaReserve != null)
            {
                _context.UserAreaReserves.Remove(userAreaReserve);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool UserAreaReserveExists(int id)
        //{
        //    return _context.UserAreaReserves.Any(e => e.Id == id);
        //}


        //tEST now 預約人數*點數=總扣點數
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("AreaId,Amount,DoorNoId,ReserveTime,StartTime,EndTime,ReservedPeople,ReservationDate,ReservationTimeSlot")]
    UserAreaReserveViewModel userAreaReserveViewModel,
    UserAreaReserve userAreaReserve)
        {
            // 先嘗試將 ReservationDate 與 ReservationTimeSlot 組合成 StartTime / EndTime
            if (!string.IsNullOrEmpty(userAreaReserveViewModel.ReservationDate) &&
                !string.IsNullOrEmpty(userAreaReserveViewModel.ReservationTimeSlot))
            {
                // ReservationTimeSlot 預期格式："08:00~09:00"
                var timeParts = userAreaReserveViewModel.ReservationTimeSlot.Split('~');
                if (timeParts.Length == 2)
                {
                    string startString = $"{userAreaReserveViewModel.ReservationDate}T{timeParts[0]}:00";
                    string endString = $"{userAreaReserveViewModel.ReservationDate}T{timeParts[1]}:00";
                    if (DateTime.TryParse(startString, out DateTime sTime))
                    {
                        userAreaReserveViewModel.StartTime = sTime;
                    }
                    if (DateTime.TryParse(endString, out DateTime eTime))
                    {
                        userAreaReserveViewModel.EndTime = eTime;
                    }
                }
            }

            // 1️⃣ 取得登入用戶的 UserId
            string? currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == currentUserId);

            if (userInfo == null)
            {
                ModelState.AddModelError("", "無法找到登入用戶資訊");
                return View(userAreaReserveViewModel);
            }

            // 2️⃣ 透過 BD_Member 表找到對應的 DoorNoId
            int? userDoorNoId = await _context.BdMembers
                .Where(m => m.UserId == userInfo.FId)
                .Select(m => m.DoorNoId)
                .FirstOrDefaultAsync();

            if (!userDoorNoId.HasValue)
            {
                ModelState.AddModelError("", "登入用戶沒有綁定門號");
                return View(userAreaReserveViewModel);
            }

            // 3️⃣ 確保 AreaId 存在
            var area = await _context.PublicAreaReserves
                .FirstOrDefaultAsync(a => a.Id == userAreaReserveViewModel.AreaId);

            if (area == null)
            {
                ModelState.AddModelError("", "選擇的公設項目不存在，請重新選擇");
                return View(userAreaReserveViewModel);
            }

            // 檢查該公共區域在指定時間段內是否已有預約
            bool isAreaAvailable = await IsAreaAvailableAsync(
                userAreaReserveViewModel.AreaId.Value,
                userAreaReserveViewModel.StartTime ?? DateTime.MinValue, // 若 StartTime 為 null，設為最小日期
                userAreaReserveViewModel.EndTime ?? DateTime.MinValue    // 若 EndTime 為 null，設為最小日期
            );
            if (!isAreaAvailable)
            {
                ModelState.AddModelError("", "該公共區域在指定時間段內的預約已滿，請選擇其他時間。");
                ViewBag.IsAreaAvailable = false;
                return View(userAreaReserveViewModel);
            }
            ViewBag.IsAreaAvailable = true;

            // 4️⃣ 取得用戶點數並計算剩餘點數
            int userBdId = (int)userInfo.FBuildingId;
            int userPoints = await GetUserPointsAsync(userBdId);
            int deductPoints = (int)area.DeductUnit;
            // 新增：計算總扣點數 = 預約人數 * 單位扣點數
            // 如果 ReservedPeople 為 null，視為 0
            int reservedPeople = userAreaReserveViewModel.ReservedPeople ?? 0;
            int totalDeduction = (userAreaReserveViewModel.ReservedPeople ?? 0) * deductPoints;
            // 設定到 ViewModel
            userAreaReserveViewModel.TotalDeduction = totalDeduction;
            int remainingPoints = userPoints - totalDeduction;
            //int remainingPoints = userPoints - deductPoints;

            if (remainingPoints < 0)
            {
                ModelState.AddModelError("", "您的點數不足，無法預約此公設");
                return View(userAreaReserveViewModel);
            }

            // 5️⃣ 檢查並計算指定時間段內剩餘可預約人數
            DateTime startTime = userAreaReserveViewModel.StartTime ?? DateTime.MinValue;
            DateTime endTime = userAreaReserveViewModel.EndTime ?? DateTime.MinValue;

            int maxSlots = area.Amount ?? 10;
            int reservedCount = await _context.UserAreaReserves
                .Where(r => r.AreaId == userAreaReserveViewModel.AreaId &&
                            r.StartTime >= startTime &&
                            r.EndTime <= endTime)
                .SumAsync(r => r.ReservedPeople ?? 0);

            int remainingSlots = maxSlots - reservedCount;

            if (userAreaReserveViewModel.ReservedPeople > remainingSlots)
            {
                ModelState.AddModelError("", $"剩餘可預約人數不足，目前僅剩 {remainingSlots} 人可預約。");
                return View(userAreaReserveViewModel);
            }

            // 將剩餘可預約名額傳回 ViewBag，以便在 View 顯示（例如下次重新載入表單時）
            ViewBag.RemainingSlots = remainingSlots;

            // 6️⃣ 設定預約資料
            userAreaReserve.UserId = userInfo.FId;
            userAreaReserve.DoorNoId = userDoorNoId.Value;
            userAreaReserve.State = 1;
            userAreaReserve.ReservedPeople = userAreaReserveViewModel.ReservedPeople.Value;

            // 確保 StartTime 和 EndTime 正確
            if (userAreaReserve.StartTime == DateTime.MinValue)
            {
                ModelState.AddModelError("", "開始時間無效");
                return View(userAreaReserveViewModel);
            }
            if (userAreaReserve.EndTime == DateTime.MinValue)
            {
                ModelState.AddModelError("", "結束時間無效");
                return View(userAreaReserveViewModel);
            }

            if (ModelState.IsValid)
            {
                // 7️⃣ 插入 UserAreaReserve 資料
                _context.Add(userAreaReserve);
                await _context.SaveChangesAsync();

                // 8️⃣ 更新 DoorNos 表格中的點數
                var doorNo = await _context.DoorNos.FirstOrDefaultAsync(d => d.Id == userDoorNoId.Value);
                if (doorNo != null)
                {
                    doorNo.Points = remainingPoints;
                    _context.Update(doorNo);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", "找不到對應的門號資料");
                    return View(userAreaReserveViewModel);
                }

                // 9️⃣ 預約成功後導向 Index 頁面
                return RedirectToAction(nameof(Index));
            }

            // 若 ModelState 有錯誤，重新顯示畫面
            ViewBag.AreaId = userAreaReserveViewModel.AreaId;
            return View(userAreaReserveViewModel);
        }

      
        private async Task<int> GetUserPointsAsync(int buildingId)
        {
            return await (from bm in _context.BdMembers
                          join dn in _context.DoorNos on bm.DoorNoId
                           equals dn.Id
                          where bm.BdId == buildingId
                          select dn.Points).FirstOrDefaultAsync() ?? 0; // 若查無點數，預設為 0
        }

       
        
        private async Task<bool> IsAreaAvailableAsync(int areaId, DateTime startTime, DateTime endTime)
        {
            // 取得該區域的最大可預約數量
            var area = await _context.PublicAreaReserves
                .Where(a => a.Id == areaId)
                .Select(a => new { a.Amount })
                .FirstOrDefaultAsync();

            if (area == null)
            {
                return false; // 若找不到該區域，則不可預約
            }

            // 計算該時間段內已經存在的預約筆數
            int existingReservations = await _context.UserAreaReserves
                .Where(r => r.AreaId == areaId &&
                           ((startTime >= r.StartTime && startTime < r.EndTime) ||
                            (endTime > r.StartTime && endTime <= r.EndTime) ||
                            (startTime <= r.StartTime && endTime >= r.EndTime)))
                .CountAsync(); // 統計該時段內的預約筆數

            // 檢查是否還有可預約名額
            return existingReservations < area.Amount;
        }

        //計算剩餘人數
        //private async Task<int> GetRemainingSlotsAsync(int areaId, DateTime startTime, DateTime endTime)
        //{
        //    // 取得該區域的最大可預約人數 (Amount)
        //    var area = await _context.PublicAreaReserves
        //        .Where(a => a.Id == areaId)
        //        .Select(a => new { a.Amount })
        //        .FirstOrDefaultAsync();

        //    if (area == null)
        //    {
        //        return 0; // 若找不到該區域，則無法預約
        //    }

        //    // 計算該時段內已經被預約的人數總和
        //    int reservedPeople = (int)await _context.UserAreaReserves
        //        .Where(r => r.AreaId == areaId &&
        //                   ((startTime >= r.StartTime && startTime < r.EndTime) ||
        //                    (endTime > r.StartTime && endTime <= r.EndTime) ||
        //                    (startTime <= r.StartTime && endTime >= r.EndTime)))
        //        .SumAsync(r => r.ReservedPeople); // 加總已預約人數

        //    // 計算剩餘可預約人數
        //    int remainingSlots = (int)(area.Amount - reservedPeople);

        //    return remainingSlots > 0 ? remainingSlots : 0; // 確保不會出現負數
        //}


      


        [HttpGet]
        public async Task<IActionResult> CancelReservation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var userAreaReserve = await _context.UserAreaReserves
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userAreaReserve == null)
            {
                return NotFound();
            }

            // 2️⃣ 查詢公設名稱（透過 `UserAreaReserve.AreaId`）
            var publicArea = await _context.PublicAreaReserves
                .FirstOrDefaultAsync(a => a.Id == userAreaReserve.AreaId);

            if (publicArea == null)
            {
                return NotFound();  // 找不到公設資料
            }

           

            // ✅ 傳遞 **公設名稱** 到 View
            ViewData["PublicAreaName"] = publicArea.Name;


            return View(userAreaReserve); // 這裡應該返回一個視圖，而不是執行刪除
        }

        [HttpPost, ActionName("CancelReservation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservationConfirmed(int id)
        {
            var userAreaReserve = await _context.UserAreaReserves.FindAsync(id);
            if (userAreaReserve == null)
            {
                return NotFound();
            }

            // 取得用戶資訊
            string? currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userInfo = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == currentUserId);
            if (userInfo == null)
            {
                return NotFound();
            }

            var userDoorNo = await _context.BdMembers
                .Where(m => m.UserId == userInfo.FId)
                .Select(m => m.DoorNoId)
                .FirstOrDefaultAsync();

            if (userDoorNo == null)
            {
                return NotFound();
            }

            var doorInfo = await _context.DoorNos.FirstOrDefaultAsync(d => d.Id == userDoorNo);
            if (doorInfo == null)
            {
                return NotFound();
            }

            var area = await _context.PublicAreaReserves
                .Where(a => a.Id == userAreaReserve.AreaId)
                .FirstOrDefaultAsync();

            if (area == null)
            {
                return NotFound();
            }
           

            // 恢復點數
            int newPoints = (int)doorInfo.Points + ((int)area.DeductUnit * (int)userAreaReserve.ReservedPeople);
            doorInfo.Points = newPoints;
            _context.Update(doorInfo);

            // 更新狀態為已取消
            userAreaReserve.State = 3;
            _context.Update(userAreaReserve);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //private async Task<Dictionary<string, int>> GetRemainingSlotsForAllTimesAsync(int areaId, DateTime date, decimal timeUnit)
        //{
        //    var area = await _context.PublicAreaReserves
        //        .Where(a => a.Id == areaId)
        //        .Select(a => new { a.Amount, a.OpenTime, a.CloseTime })
        //        .FirstOrDefaultAsync();

        //    if (area == null || !area.OpenTime.HasValue || !area.CloseTime.HasValue)
        //    {
        //        return new Dictionary<string, int>(); // 如果找不到該區域或時間範圍無效，回傳空值
        //    }

        //    var openTime = new TimeSpan(area.OpenTime.Value.Hour, area.OpenTime.Value.Minute, 0);
        //    var closeTime = new TimeSpan(area.CloseTime.Value.Hour, area.CloseTime.Value.Minute, 0);
        //    var remainingSlotsDict = new Dictionary<string, int>();

        //    var startTime = date.Date + openTime;
        //    var endTime = startTime.AddHours((double)timeUnit);

        //    while (endTime.TimeOfDay <= closeTime)
        //    {
        //        int reservedPeople = (int)await _context.UserAreaReserves
        //            .Where(r => r.AreaId == areaId &&
        //                        ((startTime >= r.StartTime && startTime < r.EndTime) ||
        //                         (endTime > r.StartTime && endTime <= r.EndTime) ||
        //                         (startTime <= r.StartTime && endTime >= r.EndTime)))
        //            .SumAsync(r => r.ReservedPeople);

        //        int remainingSlots = Math.Max((area.Amount ?? 0) - reservedPeople, 0);
        //        string timeSlot = $"{startTime:HH:mm}~{endTime:HH:mm}";

        //        remainingSlotsDict[timeSlot] = remainingSlots;

        //        startTime = endTime;
        //        endTime = startTime.AddHours((double)timeUnit);
        //    }

        //    return remainingSlotsDict;
        //}

        //AJAX--OK
        [HttpGet]
        public async Task<JsonResult> GetTimeSlots(DateTime date, int areaId)
        {
            // 取得公設資料
            var area = await _context.PublicAreaReserves.FirstOrDefaultAsync(a => a.Id == areaId);
            if (area == null)
            {
                return Json(new { success = false, message = "找不到該公設資料" });
            }

            // 設定開放與關閉時間（例如預設為 08:00 ~ 22:00，或從 area 中讀取）
            TimeSpan openTime = area.OpenTime.HasValue ? area.OpenTime.Value.ToTimeSpan() : TimeSpan.FromHours(8);
            TimeSpan closeTime = area.CloseTime.HasValue ? area.CloseTime.Value.ToTimeSpan() : TimeSpan.FromHours(22);

            // UseTimeUnit 為小時，預設 1 小時
            TimeSpan slotDuration = TimeSpan.FromHours((double)(area.UseTimeUnit ?? 1.0m));

            DateTime dayStart = date.Date + openTime;
            DateTime dayEnd = date.Date + closeTime;

            var slots = new List<TimeSlotViewModel>();
            DateTime current = dayStart;
            while (current < dayEnd)
            {
                DateTime slotStart = current;
                DateTime slotEnd = current + slotDuration;
                if (slotEnd > dayEnd)
                {
                    slotEnd = dayEnd;
                }
                string slotText = $"{slotStart:HH:mm}~{slotEnd:HH:mm}";

                // 取得該時段中已預約的人數，這裡假設 UserAreaReserves 表中有 ReservedPeople 欄位
                int reservedCount = await _context.UserAreaReserves
                    .Where(u => u.AreaId == areaId &&
                                u.ReserveTime.Value.Date == date.Date &&
                                u.StartTime >= slotStart &&
                                u.StartTime < slotEnd)
                    .SumAsync(u => (int?)u.ReservedPeople) ?? 0;

                int maxCapacity = area.Amount ?? 0;
                int remainingCapacity = maxCapacity - reservedCount;

                slots.Add(new TimeSlotViewModel
                {
                    TimePeriod = slotText,
                    StartDateTime = slotStart,
                    EndDateTime = slotEnd,
                    RemainingCapacity = remainingCapacity
                });

                current = slotEnd;
            }

            return Json(slots);
        }

        //結合AJAXTEST--OK
        public async Task<IActionResult> Create(int? areaId)
        {
            if (areaId == null)
            {
                return NotFound();
            }

            // 取得公設資料
            var area = await _context.PublicAreaReserves.FirstOrDefaultAsync(a => a.Id == areaId);
            if (area == null)
            {
                return NotFound();
            }

            // 設定公設的開放/關閉時間 (假設 area.OpenTime、area.CloseTime 為 TimeOnly? 型別)
            if (area.OpenTime.HasValue && area.CloseTime.HasValue)
            {
                ViewBag.OpenTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, area.OpenTime.Value.Hour, area.OpenTime.Value.Minute, 0).ToString("HH:mm");
                ViewBag.CloseTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, area.CloseTime.Value.Hour, area.CloseTime.Value.Minute, 0).ToString("HH:mm");
            }
            else
            {
                ViewBag.OpenTime = "00:00";
                ViewBag.CloseTime = "00:00";
            }

            // 讀取 Use_Time_Unit (單位: 小時)
            decimal timeUnit = area.UseTimeUnit ?? 1.0m;  // 預設 1 小時
            int Amount = area.Amount ?? 0;  // 公設最大可預約人數

            // 取得當前登入用戶的 TusersInfos 資料
            string? currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userInfo = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == currentUserId);
            if (userInfo == null)
            {
                return NotFound();
            }

            // 從 BD_Members 表取得 DoorNoId
            var userDoorNo = await _context.BdMembers
                .Where(m => m.UserId == userInfo.FId)
                .Select(m => m.DoorNoId)
                .FirstOrDefaultAsync();
            if (userDoorNo == null)
            {
                return NotFound("找不到用戶對應的門牌號碼。");
            }

            // 確保 Door_No 資料存在
            var doorInfo = await _context.DoorNos.FirstOrDefaultAsync(d => d.Id == userDoorNo);
            if (doorInfo == null)
            {
                return NotFound("找不到對應的 Door_No 資料");
            }

            int userBdId = (int)userInfo.FBuildingId;  // 用戶所屬大樓 ID
                                                       // 使用獨立方法取得用戶點數
            int userPoints = await GetUserPointsAsync(userBdId);

            // 每人扣點數 (扣點單位)
            int deductPoints = (int)area.DeductUnit;
            // 初始剩餘點數 (新增預約前尚未扣點，此處可先設定為用戶點數)
            int remainingPoints = userPoints;

            // 建立新增預約時的初始模型 (UserAreaReserveViewModel)
            var model = new UserAreaReserveViewModel
            {
                AreaId = area.Id,
                UserId = userInfo.FId,
                DoorNoId = (int)userDoorNo,
                State = 1, // 預設狀態，例如 1 代表預約成功待確認
                UserPoints = userPoints,
                DeductPoints = deductPoints,
                RemainingPoints = remainingPoints,
                Amount = Amount
                // 注意：其他預約相關欄位（例如 ReservedPeople、TimePeriod）將由用戶在表單中填寫，
                // 並由前端進行 AJAX 動態載入時段選項，這裡可預設 ReservedPeople 為 1
            };

            // 傳遞其他必要的資訊給 View
            ViewBag.AreaId = area.Id;
            ViewBag.AreaName = area.Name;
            ViewBag.UserId = userInfo.FId;
            ViewBag.UserPoints = userPoints;
            ViewBag.RemainingPoints = remainingPoints;
            ViewBag.DoorNoId = userDoorNo;
            ViewBag.IntervalHours = timeUnit;  // 時段間隔（小時）

            return View(model);
        }

        
        
        //AJAX+Index
        public async Task<IActionResult> Index()
        {
            // 取得當前登入的用戶資料
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

            // 取得用戶點數
            int userPoints = await GetUserPointsAsync(userBdId);

            // 取得公設資料（此部分用於顯示大樓公設）
            var publicAreas = await _context.PublicAreaReserves
                 .Where(p => p.BdId == userBdId)
                 .Select(p => new PublicAreaReserve
                 {
                     Id = p.Id,
                     Name = p.Name,
                     AreaInfo = p.AreaInfo,
                     OpenTime = p.OpenTime,
                     CloseTime = p.CloseTime
                 }).ToListAsync();

            // 這裡不必一次性取得所有預約資料，
            // 預設可讓 AJAX 在畫面載入後再載入 active 的資料

            var model = new UserAreaReserveWrap
            {
                User = user,
                PublicAreaReserves = publicAreas,
                UserPoints = userPoints
                // UserAreaReserves 可暫時不設定，由 AJAX 載入
            };

            return View(model);
        }


        //AJAX
        public async Task<IActionResult> LoadReservations(string type)
        {
            // 取得當前登入的用戶資料
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // 取得用戶的資訊及大樓ID
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound("無法找到用戶對應的大樓資訊。");
            }
            int userBdId = (int)userInfo.FBuildingId;

            // 查詢該用戶的預約資料（連帶公設資料）
            var userAreaReserves = await (from uar in _context.UserAreaReserves
                                          join par in _context.PublicAreaReserves on uar.AreaId equals par.Id
                                          where uar.UserId == userInfo.FId
                                          select new UserAreaReserveWithAreaInfo
                                          {
                                              Id = uar.Id,
                                              AreaId = uar.AreaId,
                                              DoorNoId = uar.DoorNoId,
                                              UserId = uar.UserId,
                                              ReserveTime = uar.ReserveTime,
                                              StartTime = uar.StartTime,
                                              EndTime = uar.EndTime,
                                              State = uar.State,
                                              ReservedPeople = uar.ReservedPeople,
                                              PublicArea = par
                                          }).OrderByDescending(uar => uar.ReserveTime)
                                 .ToListAsync();

            // 更新過期預約的狀態（例如：結束時間小於現在的，更新為 2）
            foreach (var reserve in userAreaReserves)
            {
                if (reserve.StartTime < DateTime.Now)
                {
                    reserve.State = 2;
                }
            }

            // 根據傳入的 type 參數篩選資料
            if (type == "active")
            {
                var activeReservations = userAreaReserves.Where(r => r.State == 1).ToList();
                return PartialView("_ActiveReservationsPartial", activeReservations);
            }
            else if (type == "history")
            {
                var historyReservations = userAreaReserves.Where(r => r.State == 2 || r.State == 3).ToList();
                return PartialView("_HistoryReservationsPartial", historyReservations);
            }
            else
            {
                return Content("無效的參數");
            }
        }


    }
}
