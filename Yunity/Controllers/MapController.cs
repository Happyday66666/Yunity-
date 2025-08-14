using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class MapController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public MapController(BuildingDataContext context, UserManager<YunityUser> userManager)
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

            // 從 BD_List 資料表中查詢對應的大樓資訊
            var building = await _context.BdLists.FirstOrDefaultAsync(b => b.Id == userBdId);
            if (building == null)
            {
                return NotFound("無法找到對應的大樓資料。");
            }

            // 可利用 ViewBag 或 ViewModel 將大樓名稱傳遞到 View
            ViewBag.BuildingName = building.BdName;
            ViewBag.BuildingAddress = building.BdAddr;
           
            return View();
        }
        public IActionResult Index2()
        {
            return View();
        }

        //廠商服務區篩選
        [HttpGet]
        public async Task<IActionResult> GetVendorCoordinates()
        {
            // 1. 獲取登入者資訊
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { error = "使用者未登入" });
            }

            // 2. 取得使用者對應的大樓 ID
            var userInfo = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound(new { error = "無法找到用戶對應的大樓資訊" });
            }

            int userBdId = (int)userInfo.FBuildingId;

            // 3. 取得大樓地址
            var building = await _context.BdLists.FirstOrDefaultAsync(b => b.Id == userBdId);
            if (building == null || string.IsNullOrEmpty(building.BdAddr))
            {
                return NotFound(new { error = "無法找到對應的大樓資料或地址" });
            }

            // 4. 提取大樓地址中的關鍵字，並返回對應的欄位名稱 (如 "KaohsiungCity")
            string regionColumn = ExtractRegionKeywordFromAddress(building.BdAddr);
            if (string.IsNullOrEmpty(regionColumn))
            {
                return NotFound(new { error = "大樓地址不包含可識別的服務區域字" });
            }

            // 5. 使用 SQL 條件檢查對應的欄位是否為 1
            var vendorData = await (
                from vc in _context.VendorCoordinates
                join cp in _context.CompanyProfiles on vc.CompanyProfileId equals cp.Id
                join ca in _context.CompanyAccounts on cp.ComId equals ca.Id
                join csa in _context.CompanyServiceAreas on cp.ComId equals csa.ComId
                where ca.ComStatus == 1 // 已開通廠商
                select new
                {
                    csa,
                    cp,
                    ca,
                    vc
                }
            )
            .Where(v => EF.Property<int>(v.csa, regionColumn) == 1) // **重點：使用 EF.Property 動態比較欄位**
            .Select(v => new
            {
                address = v.cp.ComAddress,
                phone = v.cp.ComPhone,
                serviceItem = v.cp.ComServerItem,
                businessTime = v.cp.ComBusinessHours,
                companyName = v.ca.ComName,
                latitude = Convert.ToDouble(v.vc.Latitude),    // 明確轉為 double
                longitude = Convert.ToDouble(v.vc.Longitude)   // 明確轉為 double
            })
            .ToListAsync();

            if (!vendorData.Any())
            {
                return NotFound(new { error = "找不到包含該關鍵字的廠商" });
            }

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return new JsonResult(vendorData, options);
        }


        private string ExtractRegionKeywordFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return string.Empty;
            }

            var regionKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "宜蘭", "YilanC" },
        { "基隆", "KeelungCity" },
        { "台北", "TaipeiCity" },
        { "新北", "NewTaipeiCity" },
        { "桃園", "TaoyuanCity" },
        { "新竹市", "HsinchuCity" },
        { "新竹", "HsinchuC" },  // 可同時適用於市和縣
        { "苗栗", "MiaoliC" },
        { "台中", "TaichungCity" },
        { "彰化", "ChanghuaC" },
        { "南投", "NantouC" },
        { "雲林", "YunlinC" },
        { "嘉義市", "ChiayiCity" },
        { "嘉義", "ChiayiC" },
        { "台南", "TainanCity" },
        { "高雄", "KaohsiungCity" },
        { "屏東", "PingtungC" },
        { "澎湖", "PenghuC" },
        { "花蓮", "HualienC" },
        { "台東", "TaitungC" },
        { "金門", "KinmenC" },
        { "連江", "LienchiangC" }
    };

            foreach (var keyword in regionKeywords)
            {
                if (address.Contains(keyword.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return keyword.Value;  // 返回對應的欄位名稱
                }
            }

            return string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetNearStoreCoordinates()
        {
            // 1. 獲取登入者資訊
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { error = "使用者未登入" });
            }

            // 2. 取得使用者對應的大樓 ID
            var userInfo = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound(new { error = "無法找到用戶對應的大樓資訊" });
            }

            int userBdId = (int)userInfo.FBuildingId;

            // 查詢：從 NearStoreCoordinates 與 Near_Store 及 Near_Store_withBD 三表連結過濾符合使用者大樓的記錄
            var nearStoreData = _context.NearStoreCoordinates
                .Join(_context.NearStores,
                      nsc => nsc.NearStoreId,
                      ns => ns.Id,
                      (nsc, ns) => new { nsc, ns })
                .Join(_context.NearStoreWithBds,
                      temp => temp.ns.Id,
                      nsbd => nsbd.NearStoreId,
                      (temp, nsbd) => new { temp.nsc, temp.ns, nsbd })
                .Where(x => x.nsbd.BdId == userBdId && x.nsbd.State == 1)
                .Select(x => new
                {
                    name = x.ns.Name ?? "未命名",           // 防止名稱為 null
                    phone = x.ns.NsPhone ?? "無電話",
                    address = x.ns.Addr ?? "無地址",         // 防止地址為 null
                    openTime = x.ns.OpenTime ?? "無營業時間", // 防止營業時間為 null
                    type = x.ns.Type,                      // 原始類型數值
                    photo = $"/NearStore/{x.ns.Photo}",    // 照片存放路徑格式
                    info = x.ns.Info ?? "無資訊",           // 防止資訊為 null
                    latitude = Convert.ToDouble(x.nsc.Latitude ?? 0),  // 防止經度為 null
                    longitude = Convert.ToDouble(x.nsc.Longitude ?? 0)  // 防止緯度為 null
                })
                .ToList();

            // 映射商店類型（使用 GetStoreType 方法將 int 轉為中文描述）
            var result = nearStoreData.Select(store => new
            {
                store.name,
                store.phone,
                store.address,
                store.openTime,
                type = GetStoreType((int)store.type),
                store.photo,
                store.info,
                store.latitude,
                store.longitude
            }).ToList();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return new JsonResult(result, options);
        }

        // 映射商店類型，這個方法不在 LINQ 查詢中執行
        private string GetStoreType(int type)
        {
            switch (type)
            {
                case 1: return "便利商店";
                case 2: return "咖啡廳";
                case 3: return "藥局";
                case 4: return "超市";
                case 5: return "飲料店";
                case 6: return "麵包店";
                case 7: return "餐廳";
                default: return "未知";
            }
        }


        

    }
}
