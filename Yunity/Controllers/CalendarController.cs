using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using Yunity.Data;
using Yunity.Models;

namespace Yunity.Controllers
{
    public class CalendarController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        // 返回 Partial View
        [HttpGet]
        public IActionResult LoadCalendarPartial()
        {
            return PartialView("~/Views/Calendar/_CalendarPartial.cshtml");

        }

        [HttpGet]
        public IActionResult GetEvents(DateTime? start, DateTime? end)
        {
            // 確保 start 和 end 有值，若無則使用當前月份的範圍
            DateTime monthStart = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime monthEnd = end ?? monthStart.AddMonths(1).AddDays(-1);

            // 初始化事件列表
            var events = new List<object>();

            BuildingDataContext db = new BuildingDataContext();
            // 查詢大樓合約到期資料
            var contracts = db.BdLists
                .Where(b => b.ContractEnd.HasValue) // 只過濾掉 ContractEnd 為 null 的資料
                .Select(b => new
                {
                    Day = b.ContractEnd.Value.Day,
                    Name = b.BdName,
                    ContractEnd = b.ContractEnd
                })
                .ToList();
            // 輸出查詢結果到控制台
            Console.WriteLine($"大樓合約查詢結果: {contracts.Count} 筆資料");
            foreach (var contract in contracts)
            {
                Console.WriteLine($"Name: {contract.Name}, Date: {contract.ContractEnd}");
            }

            foreach (var contract in contracts)
            {
                events.Add(new
                {
                    title = $"大樓合約到期: {contract.Name}",
                    start = contract.ContractEnd.Value.ToString("yyyy-MM-dd"),
                    end = contract.ContractEnd.Value.ToString("yyyy-MM-dd"),
                    color = "red" // 自訂顏色（可選）
                });
            }

            // 查詢廠商合約到期資料
            var query = from Company in db.CompanyProfiles
                        join Account in db.CompanyAccounts on Company.ComId equals Account.Id
                        where Company.ComContractEndDate.HasValue // 過濾掉 ComContractEndDate 為 null 的資料
                        select new
                        {
                            Company.ComContractEndDate,
                            CompName = Account.ComName
                        };

            var contractEvents = query
                .Select(c => new
                {
                    Day = c.ComContractEndDate.Value.Day,
                    Name = c.CompName,
                    ContractEnd = c.ComContractEndDate
                })
                .ToList();

            foreach (var ev in contractEvents)
            {
                events.Add(new
                {
                    title = $"廠商合約到期: {ev.Name}",
                    start = ev.ContractEnd.Value.ToString("yyyy-MM-dd"),
                    end = ev.ContractEnd.Value.ToString("yyyy-MM-dd"),
                    color = "blue" // 自訂顏色（可選）
                });
            }


            return Json(events);
        }
    }
}
