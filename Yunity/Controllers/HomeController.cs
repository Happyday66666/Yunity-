using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BuildingDataContext _BD;

        public HomeController(ILogger<HomeController> logger, BuildingDataContext context)
        {
            _logger = logger;
            _BD = context;

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // 檢查是否登入，並且不是登入頁面
            if (!IsUserLoggedIn() && !ControllerContext.ActionDescriptor.ActionName.Equals("Login") &&
    !ControllerContext.ActionDescriptor.ActionName.Equals("GetDateLineCounts"))
            {
                // 沒有登入時，重定向到登入頁
                context.Result = RedirectToAction("Login");
            }
        }
        // 檢查使用者是否登入（依據 Session 來判斷）
        private bool IsUserLoggedIn()
        {
            var loggedInUser = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return !string.IsNullOrEmpty(loggedInUser);
        }


        // 查詢大樓合作到期案件數量
        private async Task<int> getBDCount()
        {
            var today = DateTime.Today;
            var oneMonthLater = today.AddMonths(1);

            return await _BD.BdLists
                .Where(c => c.ContractEnd >= today && c.ContractEnd <= oneMonthLater)
                .CountAsync();
        }

        // 查詢廠商合作到期案件數量
        private async Task<int> getCPCount()
        {
            var today = DateTime.Today;
            var oneMonthLater = today.AddMonths(1);

            return await _BD.CompanyProfiles
                .Where(c => c.ComContractEndDate.HasValue &&
                            c.ComContractEndDate.Value.Date >= today &&
                            c.ComContractEndDate.Value.Date <= oneMonthLater)
                .CountAsync();
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //登出登入
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(CLoginViewModel vm)
        {
            TSystemInfo user = (new BuildingDataContext()).TSystemInfos.FirstOrDefault(
                t => t.FAccount.Equals(vm.txtAccount) && t.FPassword.Equals(vm.txtPassword));


            if (user != null && user.FPassword.Equals(vm.txtPassword))
            {
                CHome_Model.user_name = user.FName;
                ViewBag.user_name = CHome_Model.user_name;
                string json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
                string ISjson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "帳號或密碼錯誤。");
            return View();
        }
        //[HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_LOINGED_USER);
            return RedirectToAction("Login", "Home");

        }


        //Index
        // 顯示登入頁
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // 計算到期案件數量並傳遞到視圖
        public async Task<IActionResult> Index(string year, string month)
        {   // 登入名稱
            var userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER); // 從 Session 中取出
            var user = !string.IsNullOrEmpty(userJson)
           ? JsonSerializer.Deserialize<TSystemInfo>(userJson)
           : null;


            if (user != null)
            {
                ViewBag.LOGINED_USER = user.FAccount; // 使用者名稱放入 ViewBag
            }


            ViewBag.user_name = CHome_Model.user_name;
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
            {
                year = DateTime.Now.Year.ToString();
                month = DateTime.Now.Month.ToString("D2");
            }

            // ?算到期案件?量
            var BDCount = await getBDCount();
            var CPCount = await getCPCount();

            // ?建 CDateLineCountWrap ?例
            var count = new CDateLineCountWrap
            {
                BDCount = BDCount,
                CPCount = CPCount
            };

            // ?建 CHomeWrap ?例，并? CDateLineCountWrap 和 Calendar ?据???去
            var homeWrap = new CHomeWrap
            {
                DateLineCount = count,   // 包含到期案件?量

            };

            // 返回????? CHomeWrap ?例
            return View(homeWrap);
        }


        //月曆
        // 返回 Partial View 月曆部分視圖
        [HttpGet]
        public IActionResult LoadCalendarPartial()
        {
            return PartialView("~/Views/Home/_CalendarPartial.cshtml");

        }



        //[AllowAnonymous] // 跳過身份驗證檢查
        [HttpGet]
        public async Task<IActionResult> GetDateLineCounts()
        {
            try
            {
                // 查詢大樓到期案件數量
                var bdCount = await getBDCount();

                // 查詢廠商到期案件數量
                var cpCount = await getCPCount();

                // 返回 JSON 結果
                return Json(new { bdCount, cpCount });
            }
            catch (Exception ex)
            {
                // 記錄異常（可換為真實的日誌記錄工具）
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return StatusCode(500, "伺服器內部錯誤");
            }
        }

    }
}
