using Microsoft.AspNetCore.Mvc;
using Yunity.Services;

namespace Yunity.Controllers
{
    public class GeocodingController : Controller
    {

        private readonly BatchGeocodingService _batchService;

        public IActionResult ManualUpdate()
        {
            return View();
        }

       

        public GeocodingController(BatchGeocodingService batchService)
        {
            _batchService = batchService;
        }

        // ------------------ Vendor (廠商) ------------------
        [HttpPost]
        public async Task<IActionResult> UpdateVendorCoordinatesManually(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                TempData["StatusMessage"] = "請選擇開始日期。";
                return RedirectToAction("ManualUpdate");  // 修改這裡
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            try
            {
                int count = await _batchService.UpdateVendorCoordinatesAsync(startDate.Value, endDate.Value);
                TempData["StatusMessage"] = $"廠商手動更新完成，更新筆數：{count}";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"廠商手動更新失敗：{ex.Message}";
            }

            return RedirectToAction("ManualUpdate");  // 修改這裡
        }

        // ------------------ NearStore (周邊商家) ------------------
        [HttpPost]
        public async Task<IActionResult> UpdateNearStoreCoordinatesManually(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                TempData["StatusMessage"] = "請選擇開始日期。";
                return RedirectToAction("ManualUpdate");  // 修改這裡
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            try
            {
                int count = await _batchService.UpdateNearStoreCoordinatesAsync(startDate.Value, endDate.Value);
                TempData["StatusMessage"] = $"周邊商家手動更新完成，更新筆數：{count}";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"周邊商家手動更新失敗：{ex.Message}";
            }

            return RedirectToAction("ManualUpdate");  // 修改這裡
        }




    }

}




