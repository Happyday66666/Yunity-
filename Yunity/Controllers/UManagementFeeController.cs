using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class UManagementFeeController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public UManagementFeeController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> List(string type, string payType, string startDate, string endDate)
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

            var doorNoId = db.BdMembers
                .Where(mg => mg.BdId == userBdId)
                .Select(mg => mg.DoorNoId)
                .FirstOrDefault();

            if (doorNoId == 0 || doorNoId == null)
            {
                return View("Error");
            }

            var managementFeesQuery = db.ManageFees
                .Where(fee => fee.DoorNoId == doorNoId)
                .OrderByDescending(fee => fee.Id)
                .AsQueryable();

           
            if (type == "current")
            {
                managementFeesQuery = managementFeesQuery.Where(fee => fee.State == 0);  
            }

            
            if (!string.IsNullOrEmpty(payType))
            { // 根據付款方式 0=現金，1=信用卡
                if (payType == "現金")
                {
                    managementFeesQuery = managementFeesQuery.Where(fee => fee.PayType == 0);   
                }
                else if (payType == "信用卡")
                {
                    managementFeesQuery = managementFeesQuery.Where(fee => fee.PayType == 1);   
                }
            }
            // 根據日期範圍篩選（可選）
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start = DateTime.Parse(startDate);
                managementFeesQuery = managementFeesQuery.Where(fee => fee.FeeEnd >= start);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end = DateTime.Parse(endDate);
                managementFeesQuery = managementFeesQuery.Where(fee => fee.FeeEnd <= end);
            }


            var managementFees = managementFeesQuery
                .Select(fee => new ManageFeeViewModel
                {
                    Id = fee.Id,
                    FeeName = fee.FeeName,
                    Price = fee.Price,
                    MotorPrice = fee.MotorPrice,
                    CarPrice = fee.CarPrice,
                    FeeEnd = fee.FeeEnd,
                    PayType = fee.PayType,
                    PayTime = fee.PayTime,
                    State = fee.State
                })
                .ToList();

            decimal totalAmountDue = managementFees.Sum(fee =>
                (fee.Price ?? 0) + (fee.MotorPrice ?? 0) + (fee.CarPrice ?? 0));

             
            ViewBag.TotalAmountDue = totalAmountDue;
            ViewBag.Type = type;



            if (Request.IsAjaxRequest())  
            {
                return PartialView("_ManageFeeList", managementFees);
            }

            return View(managementFees);
        }

        public async Task<IActionResult> PayAll()
        {
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

            var doorNoId = _context.BdMembers
                .Where(mg => mg.UserId == userBdId)
                .Select(mg => mg.DoorNoId)
                .FirstOrDefault();

            if (doorNoId == 0 || doorNoId == null)
            {
                return View("Error");
            }

            // 只更新當前用戶的未繳費項目
            var managementFees = _context.ManageFees
                .Where(fee => fee.DoorNoId == doorNoId && fee.State == 0)
                .ToList();

            // 更新該用戶所有未繳費的資料
            foreach (var fee in managementFees)
            {
                fee.State = 1;  // 設置為已繳費
            }

            // 儲存變更
            await _context.SaveChangesAsync();

            // 重定向回列表頁面
            return RedirectToAction("List", new { type = "current" });
        }

       
       
         








    }
}
