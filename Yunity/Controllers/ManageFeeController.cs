using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Collections.Generic;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class ManageFeeController : Controller
    {
        private readonly BuildingDataContext _context;
        public ManageFeeController(BuildingDataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ManageFee_List([FromBody] CKeywordViewModel key)
        {
            var keyword = key.Keyword;

            var MF_Name = _context.ManageFees.GroupBy(f => new { f.FeeName, f.LogTime, f.FeeEnd })
                .Select(g => new
                {
                    g.Key.FeeName,
                    g.Key.LogTime,
                    g.Key.FeeEnd,
                    TotalPrice = g.Sum(x => x.Price + x.MotorPrice + x.CarPrice),
                    PayPrice = g.Where(x => x.State == 1).Sum(x => x.Price + x.MotorPrice + x.CarPrice),
                    PayUser = g.Count(x => x.State == 1),
                    FeeName_Count = g.Count()
                });
                
            if (!string.IsNullOrEmpty(keyword))
            {
                MF_Name = MF_Name.Where(p =>
                    p.FeeName.Contains(keyword) ||
                    p.LogTime.ToString().Contains(keyword)
                );
            }

            MF_Name = MF_Name.OrderByDescending(p => p.LogTime);
            return Json(MF_Name);
        }

        [HttpPost]
        public IActionResult ManageFee_Detail([FromBody] CKeywordViewModel key)
        {
            var manageFeeData = (from f in _context.ManageFees
                                 join d in _context.DoorNos on f.DoorNoId equals d.Id into doorGroup
                                 from door in doorGroup.DefaultIfEmpty()
                                 where f.FeeName == key.MFeeName
                                 select new ManageFeeWrap
                                 {
                                     ManageFee = f,
                                     DoorName = door.DoorNo1,
                                     TotalPrice = f.Price + f.MotorPrice + f.CarPrice,
                                     Pay_Time = f.PayTime.HasValue ? f.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "-",
                                     PayType = f.PayType,
                                     State = f.State,
                                     Pay_Type = f.PayType == 0 ? "現金" : f.PayType == 1 ? "信用卡" : "-",
                                     Pay_State = f.State == 0 ? "未繳" : "已繳"
                                 }).ToList();

            if (!string.IsNullOrEmpty(key.Keyword))
            {
                manageFeeData = manageFeeData.Where(p =>
                    p.DoorName.Contains(key.Keyword) ||
                    p.TotalPrice.ToString().Contains(key.Keyword) ||
                     p.PayTime.ToString().Contains(key.Keyword)
                ).ToList();
            }

            return PartialView("ManageFee_Detail", manageFeeData);
        }

        [HttpPost]
        public IActionResult Create([FromForm] ManageFeeWrap CreateManageFee)
        {
            try
            {
                var BD_DoorNos = _context.DoorNos.Where(p => p.BdId == 1).ToList();
                if (CreateManageFee == null || !BD_DoorNos.Any())
                    return Json(new { success = false, message = "無效資料或無對應門牌號" });

                var manageFees = new List<ManageFee>();

                foreach (var door in BD_DoorNos)
                {
                    var manageFee = new ManageFee
                    {
                        Id = CreateManageFee.Id,
                        FeeName = CreateManageFee.FeeName,
                        DoorNoId = door.Id,
                        Price = (int)Math.Round((CreateManageFee.MF_Pricce ?? 0) * (door.SquareFeet ?? 0)),
                        MotorPrice = CreateManageFee.MF_Motor * door.MotorPark,
                        CarPrice = CreateManageFee.MF_Car * door.CarPark,
                        FeeEnd = CreateManageFee.FeeEnd,
                        LogTime = DateTime.Now,
                        State = 0,
                    };
                    manageFees.Add(manageFee);
                }

                // 修正這裡，使用 AddRange 批量新增
                _context.ManageFees.AddRange(manageFees);
                _context.SaveChanges();

                return Json(new { success = true, message = $"成功新增 {manageFees.Count} 筆資料！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "資料有誤，請檢查後再提交。" });
            }
        }

        [HttpPost]
        public IActionResult Edit([FromForm] ManageFeeWrap updatedManageFee)
        {
            try
            {
                if (updatedManageFee == null)
                    return Json(new { success = false, message = "無效資料" });

                var existingMF = _context.ManageFees.FirstOrDefault(p => p.Id == updatedManageFee.Id);
                if (existingMF == null)
                    return Json(new { success = false, message = "未找到對應的包裹" });
                existingMF.PayType = updatedManageFee.PayType;
                existingMF.PayTime = updatedManageFee.PayTime;
                existingMF.State = updatedManageFee.State;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
