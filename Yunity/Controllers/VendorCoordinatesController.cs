using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yunity.Models;
using Yunity.Services;

namespace Yunity.Controllers
{
    public class VendorCoordinatesController : Controller
    {

        private readonly BatchGeocodingService _batchService;

        public VendorCoordinatesController(BatchGeocodingService batchService)
        {
            _batchService = batchService;
        }

        // 顯示管理頁面及狀態訊息
        public IActionResult Index()
        {
            ViewBag.StatusMessage = TempData["StatusMessage"] as string;
            return View();
        }
      

        // 手動觸發更新經緯度 (非同步更新)，並依據指定日期範圍更新
        [HttpPost]
        public async Task<IActionResult> UpdateVendorCoordinatesManually(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                TempData["StatusMessage"] = "請選擇開始日期。";
                return RedirectToAction("Index");
            }

            // 若未提供 endDate 則預設為現在
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            try
            {
                // 假設 _batchService.UpdateVendorCoordinatesAsync 方法已修改，可接受日期範圍參數
                int count = await _batchService.UpdateVendorCoordinatesAsync(startDate.Value, endDate.Value);
                TempData["StatusMessage"] = $"手動更新完成，更新筆數：{count}";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"手動更新失敗：{ex.Message}";
            }
            return RedirectToAction("Index");
        }



    }
}
