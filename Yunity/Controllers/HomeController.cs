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

            // �ˬd�O�_�n�J�A�åB���O�n�J����
            if (!IsUserLoggedIn() && !ControllerContext.ActionDescriptor.ActionName.Equals("Login") &&
    !ControllerContext.ActionDescriptor.ActionName.Equals("GetDateLineCounts"))
            {
                // �S���n�J�ɡA���w�V��n�J��
                context.Result = RedirectToAction("Login");
            }
        }
        // �ˬd�ϥΪ̬O�_�n�J�]�̾� Session �ӧP�_�^
        private bool IsUserLoggedIn()
        {
            var loggedInUser = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return !string.IsNullOrEmpty(loggedInUser);
        }


        // �d�ߤj�ӦX�@����ץ�ƶq
        private async Task<int> getBDCount()
        {
            var today = DateTime.Today;
            var oneMonthLater = today.AddMonths(1);

            return await _BD.BdLists
                .Where(c => c.ContractEnd >= today && c.ContractEnd <= oneMonthLater)
                .CountAsync();
        }

        // �d�߼t�ӦX�@����ץ�ƶq
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


        //�n�X�n�J
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

            ModelState.AddModelError("", "�b���αK�X���~�C");
            return View();
        }
        //[HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_LOINGED_USER);
            return RedirectToAction("Login", "Home");

        }


        //Index
        // ��ܵn�J��
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // �p�����ץ�ƶq�öǻ������
        public async Task<IActionResult> Index(string year, string month)
        {   // �n�J�W��
            var userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER); // �q Session �����X
            var user = !string.IsNullOrEmpty(userJson)
           ? JsonSerializer.Deserialize<TSystemInfo>(userJson)
           : null;


            if (user != null)
            {
                ViewBag.LOGINED_USER = user.FAccount; // �ϥΪ̦W�٩�J ViewBag
            }


            ViewBag.user_name = CHome_Model.user_name;
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
            {
                year = DateTime.Now.Year.ToString();
                month = DateTime.Now.Month.ToString("D2");
            }

            // ?�����ץ�?�q
            var BDCount = await getBDCount();
            var CPCount = await getCPCount();

            // ?�� CDateLineCountWrap ?��
            var count = new CDateLineCountWrap
            {
                BDCount = BDCount,
                CPCount = CPCount
            };

            // ?�� CHomeWrap ?�ҡA�}? CDateLineCountWrap �M Calendar ?�u???�h
            var homeWrap = new CHomeWrap
            {
                DateLineCount = count,   // �]�t����ץ�?�q

            };

            // ��^????? CHomeWrap ?��
            return View(homeWrap);
        }


        //���
        // ��^ Partial View ��䳡������
        [HttpGet]
        public IActionResult LoadCalendarPartial()
        {
            return PartialView("~/Views/Home/_CalendarPartial.cshtml");

        }



        //[AllowAnonymous] // ���L���������ˬd
        [HttpGet]
        public async Task<IActionResult> GetDateLineCounts()
        {
            try
            {
                // �d�ߤj�Ө���ץ�ƶq
                var bdCount = await getBDCount();

                // �d�߼t�Ө���ץ�ƶq
                var cpCount = await getCPCount();

                // ��^ JSON ���G
                return Json(new { bdCount, cpCount });
            }
            catch (Exception ex)
            {
                // �O�����`�]�i�����u�ꪺ��x�O���u��^
                Console.WriteLine($"�o�Ϳ��~: {ex.Message}");
                return StatusCode(500, "���A���������~");
            }
        }

    }
}
