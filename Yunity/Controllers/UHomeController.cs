using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.Services;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
     
    public class UHomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;
         private readonly WeatherService _weatherService;
        public UHomeController(BuildingDataContext context, UserManager<YunityUser> userManager, WeatherService weatherService, HttpClient httpClient)
        {
            _context = context;
            _userManager = userManager;
            _weatherService = weatherService;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index原版()
        {
            BuildingDataContext db = new BuildingDataContext();

            var user = await _userManager.GetUserAsync(User);
            // 若找不到用戶或用戶角色不為 "User"，則回傳 Challenge
            if (user == null || user.Role != "User")
            {
                return Challenge();
            }

            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null)
            {
                return Challenge(); // 若 TManagerInfo 為 null，則要求登入或拒絕存取
            }

            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound("無法找到用戶對應的大樓資訊。");
            }
           
            int userBdId = (int)userInfo.FBuildingId;


            var query = from t in _context.Boards
                        join bd in _context.BdLists on t.BdId equals bd.Id into bdGroup
                        from bd in bdGroup.DefaultIfEmpty()
                        where t.BdId == userBdId && t.State == 3
                        select new Yunity.Models.CBoardwarp
                        {
                            Id = t.Id,
                            BdId = t.BdId,
                            BdName = bd.BdName,
                            Name = t.Name,
                            Info = t.Info,
                            Photo = "/boardPhoto/" + t.Photo,
                            OpenTime = t.OpenTime,
                            State = t.State,
                            Type = t.Type
                        };



            var filteredData = query.ToList();
            return View(filteredData);
        }

        public async Task<IActionResult> Index(double? lat, double? lon)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Role != "User")
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

            var query = from t in _context.Boards
                        join bd in _context.BdLists on t.BdId equals bd.Id into bdGroup
                        from bd in bdGroup.DefaultIfEmpty()
                        where t.BdId == userBdId && t.State == 3
                        select new Yunity.Models.CBoardwarp
                        {
                            Id = t.Id,
                            BdId = t.BdId,
                            BdName = bd.BdName,
                            Name = t.Name,
                            Info = t.Info,
                            Photo = "/boardPhoto/" + t.Photo,
                            OpenTime = t.OpenTime,
                            State = t.State,
                            Type = t.Type
                        };

            var filteredData = query.ToList();

            // ✅ 嘗試獲取最近測站的天氣資訊
            Station? nearestStation = null;
            if (lat.HasValue && lon.HasValue)
            {
                nearestStation = await _weatherService.GetNearestWeatherStationAsync(lat.Value, lon.Value);
            }

            ViewBag.Weather = nearestStation; // ✅ 傳遞天氣資訊到 View
            return View(filteredData);
        }


        [HttpGet]
        public async Task<IActionResult> GetWeatherData_原來(double lat, double lon)
        {
            // ✅ 取得最近測站的即時天氣 (`O-A0003-001`)
            Station? nearestStation = await _weatherService.GetNearestWeatherStationAsync(lat, lon);
            if (nearestStation == null)
            {
                return Json(new { success = false, message = "無法獲取即時天氣資訊" });
            }

            string countyName = nearestStation.GeoInfo?.CountyName ?? "未知縣市";

            // ✅ 取得 36 小時天氣描述 (`F-C0032-001`) + 圖示代碼 (`Wx`)
            var (weatherDescription, iconCode) = await _weatherService.GetWeatherDescriptionAsync(countyName);

            // ✅ 取得降雨機率 (`PoP`)
            int? rainProbability = await _weatherService.GetRainProbabilityAsync(countyName);

            return Json(new
            {
                success = true,
                countyName = countyName, // ✅ 只回傳縣市
                weatherDescription = weatherDescription, // ✅ 36 小時天氣描述
                iconCode = iconCode, // ✅ 天氣圖示代碼
                temperature = nearestStation.WeatherElement?.AirTemperature,
                humidity = nearestStation.WeatherElement?.RelativeHumidity,
                rainProbability = rainProbability ?? 0 // ✅ 如果沒有數據，預設為 0%
            });
        }

        //Test
        [HttpGet]
        public async Task<IActionResult> GetWeatherData_Test(double lat, double lon)
        {
            Station? nearestStation = await _weatherService.GetNearestWeatherStationAsync(lat, lon);
            if (nearestStation == null)
            {
                return new JsonResult(new { success = false, message = "無法獲取即時天氣資訊" })
                {
                    StatusCode = 200,
                    ContentType = "application/json"
                };
            }

            string countyName = nearestStation.GeoInfo?.CountyName ?? "未知縣市";
            var (weatherDescription, iconCode) = await _weatherService.GetWeatherDescriptionAsync(countyName);
            int? rainProbability = await _weatherService.GetRainProbabilityAsync(countyName);

            return new JsonResult(new
            {
                success = true,
                countyName,
                weatherDescription,
                iconCode,
                temperature = nearestStation.WeatherElement?.AirTemperature,
                humidity = nearestStation.WeatherElement?.RelativeHumidity,
                rainProbability = rainProbability ?? 0
            })
            {
                StatusCode = 200,
                ContentType = "application/json"
            };
        }


        //地址去查
        [HttpGet]
        public async Task<IActionResult> GetWeatherData(double lat, double lon)
        {
            // 第一步：使用經緯度反查地址（高雄市 鳳山區）
            var location = await _weatherService.ReverseGeocodeToCountyAndTownAsync(lat, lon);
            if (location == null)
            {
                return new JsonResult(new { success = false, message = "無法反查地理位置" });
            }

            string county = location.County ?? "未知縣市";
            string town = location.Town ?? "未知區";

            // 第二步：比對最近符合「縣市 + 區」的氣象站
            Station? nearestStation = await _weatherService.GetNearestWeatherStationByCountyAndTownAsync(lat, lon);
            if (nearestStation == null)
            {
                return new JsonResult(new { success = false, message = "無法獲取即時天氣資訊" });
            }

            // 第三步：再以「縣市」查天氣描述 + 降雨機率
            var (weatherDescription, iconCode) = await _weatherService.GetWeatherDescriptionAsync(county);
            int? rainProbability = await _weatherService.GetRainProbabilityAsync(county);

            // 第四步：回傳前端 JSON
            return new JsonResult(new
            {
                success = true,
                countyName = county,
                townName = town,
                weatherDescription,
                iconCode,
                temperature = nearestStation.WeatherElement?.AirTemperature,
                humidity = nearestStation.WeatherElement?.RelativeHumidity,
                rainProbability = rainProbability ?? 0
            });
        }





        //Get:USendPack/FilterAjax
        [HttpGet]
        public IActionResult FilterAjax(int state)
        {
            BuildingDataContext db = new BuildingDataContext();

            // 根據state過濾
            IQueryable<Yunity.Models.CBoardwarp> filteredBoards;

            if (state == -1)
            {
                // 如果是-1，代表不篩選，顯示全部
                filteredBoards = from t in db.Boards
                                 join bd in db.BdLists on t.BdId equals bd.Id into bdGroup
                                 from bd in bdGroup.DefaultIfEmpty()
                                 where t.State == 3 // 你可能需要根據不同的需求來過濾其他條件
                                 select new Yunity.Models.CBoardwarp
                                 {
                                     Id = t.Id,
                                     BdId = t.BdId,
                                     BdName = bd.BdName,
                                     Name = t.Name,
                                     Info = t.Info,
                                     Photo = "/boardPhoto/" + t.Photo,
                                     OpenTime = t.OpenTime,
                                     State = t.State,
                                     Type = t.Type
                                 };
            }
            else
            {
                // 根據state來過濾
                filteredBoards = from t in db.Boards
                                 join bd in db.BdLists on t.BdId equals bd.Id into bdGroup
                                 from bd in bdGroup.DefaultIfEmpty()
                                 where t.State == 3 && t.Type == state // 根據Type來過濾
                                 select new Yunity.Models.CBoardwarp
                                 {
                                     Id = t.Id,
                                     BdId = t.BdId,
                                     BdName = bd.BdName,
                                     Name = t.Name,
                                     Info = t.Info,
                                     Photo = "/boardPhoto/" + t.Photo,
                                     OpenTime = t.OpenTime,
                                     State = t.State,
                                     Type = t.Type
                                 };
            }

            // 執行查詢並轉換為列表
            var result = filteredBoards.ToList();

            // 返回部分視圖，這裡假設部分視圖是 "_BoardList"
            return PartialView("_BoardList", result);
        }
        public IActionResult Details(int? id)
        {
            BuildingDataContext db = new BuildingDataContext();
            //分頁

            //找資料
            var board = db.Boards
                .Where(t => t.Id == id.Value)
                .Join(db.BdLists, t => t.BdId, bd => bd.Id, (t, bd) => new
                {
                    t.Id,
                    t.BdId,
                    BdName = bd.BdName,
                    t.Name,
                    t.Info,
                    t.Photo,
                    t.OpenTime,
                    t.State,
                    t.Type
                }).FirstOrDefault();

            if (board == null)
            {
                return NotFound();  // 如果找不到資料，返回404頁面
            }
            var boardDetails = new Yunity.Models.CBoardwarp
            {
                Id = board.Id,
                BdId = board.BdId,
                BdName = board.BdName,
                Name = board.Name,
                Info = board.Info,
                Photo = "/boardPhoto/" + board.Photo,
                OpenTime = board.OpenTime,
                State = board.State,
                Type = board.Type
            };
            return View(boardDetails);



        }

         
        //布告欄
        //GET:Concierge/GetBoardList
        [HttpGet]
        public JsonResult GetBoardList()
        {
            var boards = _context.Boards
                .Where(b=>b.State==3)
                .OrderByDescending(b => b.OpenTime)
                .Take(4)//前五筆資料
                .Select(b => new
            {
                b.Id,
                b.Type,
                b.Name,
                Time = b.OpenTime
            }).ToList();

            return Json(boards);
        }
         
    }
}
