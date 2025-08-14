using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Controllers
{
    [Route("VendorSide/[action]")]
    public class VendorSideController : Controller
    {
        public class UserCompanyViewModel
        {
            public CompanyAccount? Account { get; set; }
            public CompanyProfile? Profile { get; set; }
            public CompanyServiceArea? ServiceArea { get; set; }
        }
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public VendorSideController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //List--------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> List(string orderSort, string searchInput)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // 未登入時，要求登入
            }
           // var userEmail = User.Identity.Name;
            var userAccount = _context.CompanyAccounts
                .FirstOrDefault(a => a.FAspUserId == user.Id);

            // 如果找不到对应的厂商账户
            if (userAccount == null)
            {
                return Challenge(); // 未登入時，要求登入
            }
           

            var userID = userAccount.Id;
            var findPIDs = _context.CsProducts
                .Where(a => a.ComId == userID)
                .Select(a => a.Id)
                .ToList();

            var data = _context.CsAppointmentRecords
                .Where(a => findPIDs.Contains((int)a.ProductId))
                .OrderBy(record =>
                    record.Status == "進行中" ? 1 :
                    record.Status == "已送單" ? 2 :
                    record.Status == "已完成" ? 3 :
                    record.Status == "已取消" ? 4 : 5)
                .ToList();

            return View(data);
        }
        [HttpPost]
        public IActionResult ListAjax(string orderSort, string searchInput)
        {
            var userEmail = User.Identity.Name;
            var userAccount = _context.CompanyAccounts
                .FirstOrDefault(a => a.ComAccount == userEmail);
            var userID = userAccount.Id;
            var findPIDs = _context.CsProducts
                .Where(a => a.ComId == userID)
                .Select(a => a.Id)
                .ToList();

            var data = _context.CsAppointmentRecords
                .Where(a => findPIDs.Contains((int)a.ProductId));
            // Handle search functionality (fuzzy search)
            if (!string.IsNullOrEmpty(searchInput))
            {
                data = data.Where(record => record.CustomerName.ToLower().Contains(searchInput.ToLower()) ||
                                             record.Phone.ToLower().Contains(searchInput.ToLower()) ||
                                             record.ServiceLocation.ToLower().Contains(searchInput.ToLower()));
            }

            // Handle sorting functionality
            switch (orderSort)
            {
                case "1": // Sort by status
                    data = data.OrderBy(record =>
                        record.Status == "進行中" ? 1 :
                        record.Status == "已送單" ? 2 :
                        record.Status == "已完成" ? 3 :
                        record.Status == "已取消" ? 4 : 5);
                    break;
                case "2": // Sort by CreatedDate descending (new to old)
                    data = data.OrderByDescending(record => record.CreatedDate);
                    break;
                case "3": // Sort by CreatedDate ascending (old to new)
                    data = data.OrderBy(record => record.CreatedDate);
                    break;
                default:
                    data = data.OrderBy(record => record.CreatedDate);
                    break;
            }


            return PartialView("_ListPartial", data.ToList());
        }

        //Detail----------------------------------------------
        public List<string> GetPhotoUrlsByOrderId(int orderId)
        {
            var photoIds = _context.CsOrderPhotos
                                      .Where(p => p.OrderId == orderId)
                                      .Select(p => p.PhotoId)
                                      .ToList();

            var photoUrls = photoIds.Select(photoId => $"/OrderPhoto/{photoId}").ToList();
            return photoUrls;
        }
        public IActionResult Detail(int id)
        {
            var data = _context.CsAppointmentRecords.Find(id);
            if (data == null)
            {
                return NotFound();
            }
            //find category name
            ViewBag.CategoryName = (
                from ar in _context.CsAppointmentRecords
                join p in _context.CsProducts on ar.ProductId equals p.Id
                where ar.Id == id
                select p.PCategory
            ).FirstOrDefault();

            //show imgs
            var photoUrls = GetPhotoUrlsByOrderId(data.Id);
            ViewBag.PhotoUrls = photoUrls;

            return View(data);
        }

        [HttpPost]
        public JsonResult UpdateStatus(int id, string status)
        {
            try
            {
                // Find the appointment by id
                var appointment = _context.CsAppointmentRecords.Find(id);
                if (appointment != null)
                {
                    if (status == "已完成" || status == "已取消")
                    {
                        appointment.FinishDate = DateTime.Now;
                    }
                    // Update the status
                    appointment.Status = status;
                    _context.SaveChanges();

                    // Return a success message and the updated status
                    return Json(new { success = true, updatedStatus = status });
                }
                else
                {
                    return Json(new { success = false, message = "Appointment not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //Product----------------------------------------------------------
        [HttpGet]
        public IActionResult Product()
        {
            var userEmail = User.Identity.Name;
            ViewBag.userProfile = _context.CompanyProfiles
                .FirstOrDefault(a => a.ComEmail == userEmail);



            return View();
        }
        [HttpPost]
        public IActionResult SaveProduct(string pName, string pDescription, decimal pPrice, int pStock, string pStatus, IFormFile pImage)
        {
            try
            {
                // 取得當前登入的使用者
                var userEmail = User.Identity.Name;
                var userProfile = _context.CompanyProfiles
                    .FirstOrDefault(a => a.ComEmail == userEmail);

                if (userProfile == null)
                {
                    return Json(new { success = false, message = "找不到使用者資料" });
                }

                // 儲存新產品
                var product = new CsProduct
                {
                    ComId = userProfile.ComId,
                    PCategory = userProfile.ComServerItem,
                    PName = pName,
                    PDescription = pDescription,
                    PPrice = pPrice,
                    PStock = pStock,
                    PStatus = pStatus
                };

                // 儲存產品資料
                _context.CsProducts.Add(product);
                _context.SaveChanges(); // 此時會生成並保存自動遞增的 product.Id

                // 取得剛剛新增的產品的 Id
                var productId = product.Id;

                // 如果有圖片，處理圖片儲存
                if (pImage != null && pImage.Length > 0)
                {
                    // 取得今天的日期
                    var todayDate = DateTime.Now.ToString("yyyyMMdd");

                    // 圖片檔名格式：[產品ID]_[今天日期] + 檔案副檔名
                    var fileName = $"{productId}_{todayDate}{Path.GetExtension(pImage.FileName)}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductPhoto");

                    // 確保資料夾存在
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pImage.CopyTo(stream);
                    }

                    //// 儲存圖片路徑到產品資料庫
                    //product.PImage = fileName;  // 儲存圖片名稱或相對路徑
                    _context.SaveChanges(); // 更新產品資料庫中的圖片欄位

                    // 儲存圖片資訊到 CS_Order_Photo 表
                    var orderPhoto = new CsProductPhoto
                    {
                        ProductId = productId, // 這裡使用產品ID作為order_id
                        PhotoId = fileName    // 儲存圖片的檔名作為photo_id
                    };

                    // 將圖片資料插入到 CS_Order_Photo 表
                    _context.CsProductPhotos.Add(orderPhoto);
                    _context.SaveChanges(); // 儲存圖片資料
                }

                return Json(new { success = true, message = "產品儲存成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "儲存失敗：" + ex.Message });
            }
        }



        //Setting----------------------------------------------------------


        [HttpGet]
        public IActionResult Setting()
        {
            var userEmail = User.Identity.Name;
            var userAccount = _context.CompanyAccounts
                .FirstOrDefault(a => a.ComAccount == userEmail);

            var userName = userAccount.ComName;
            ViewBag.UserName = userName;

            // 預設值
            string CStatus = "";
            string SArea = "";

            // 確保 userAccount 不為 null
            if (userAccount != null)
            {
                var userProfile = _context.CompanyProfiles.FirstOrDefault(p => p.ComEmail == userAccount.ComAccount);
                var userSArea = _context.CompanyServiceAreas.FirstOrDefault(p => p.ComId == userAccount.Id);

                var viewModel = new UserCompanyViewModel
                {
                    Account = userAccount,
                    Profile = userProfile,
                    ServiceArea = userSArea
                };

                // 計算狀態
                CStatus = (userAccount.ComStatus == 0) ? "停用" : "正常";

                // 計算服務區域
                if (userSArea != null)
                {
                    if (userSArea.YilanC == 1) SArea += "宜蘭縣、";
                    if (userSArea.KeelungCity == 1) SArea += "基隆市、";
                    if (userSArea.TaipeiCity == 1) SArea += "臺北市、";
                    if (userSArea.NewTaipeiCity == 1) SArea += "新北市、";
                    if (userSArea.TaoyuanCity == 1) SArea += "桃園市、";
                    if (userSArea.HsinchuCity == 1) SArea += "新竹市、";
                    if (userSArea.HsinchuC == 1) SArea += "新竹縣、";
                    if (userSArea.MiaoliC == 1) SArea += "苗栗縣、";
                    if (userSArea.TaichungCity == 1) SArea += "臺中市、";
                    if (userSArea.ChanghuaC == 1) SArea += "彰化縣、";
                    if (userSArea.NantouC == 1) SArea += "南投縣、";
                    if (userSArea.YunlinC == 1) SArea += "雲林縣、";
                    if (userSArea.ChiayiCity == 1) SArea += "嘉義市、";
                    if (userSArea.ChiayiC == 1) SArea += "嘉義縣、";
                    if (userSArea.TainanCity == 1) SArea += "臺南市、";
                    if (userSArea.KaohsiungCity == 1) SArea += "高雄市、";
                    if (userSArea.PingtungC == 1) SArea += "屏東縣、";
                    if (userSArea.PenghuC == 1) SArea += "澎湖縣、";
                    if (userSArea.HualienC == 1) SArea += "花蓮縣、";
                    if (userSArea.TaitungC == 1) SArea += "臺東縣、";
                    if (userSArea.KinmenC == 1) SArea += "金門縣、";
                    if (userSArea.LienchiangC == 1) SArea += "連江縣、";
                }

                // 如果 SArea 為空，顯示 "無"
                if (string.IsNullOrEmpty(SArea))
                {
                    SArea = "無";
                }

                // 設定 ViewBag
                ViewBag.userInfo = viewModel;
                ViewBag.CStatus = CStatus;
                ViewBag.SArea = SArea;
            }
            else
            {
                // 處理找不到用戶的情況
                ViewBag.userInfo = null;
                ViewBag.CStatus = "無資料";
                ViewBag.SArea = "無";
            }

            return View();
        }
        [HttpPost]
        public ActionResult UpdateCompanyInfo([FromBody] UserCompanyViewModel updatedUserInfo)
        {
            try
            {
                var existingProfile = _context.CompanyProfiles.FirstOrDefault(c => c.ComId == updatedUserInfo.Profile.ComId);
                if (existingProfile != null)
                {
                    existingProfile.ComPerson = updatedUserInfo.Profile.ComPerson;
                    existingProfile.ComPhone = updatedUserInfo.Profile.ComPhone;
                    existingProfile.ComAddress = updatedUserInfo.Profile.ComAddress;
                    existingProfile.ComBusinessHours = updatedUserInfo.Profile.ComBusinessHours;
                    existingProfile.ComModifyTime = updatedUserInfo.Profile.ComModifyTime;

                    int result = _context.SaveChanges();
                    if (result > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "更新沒有生效" });
                    }
                }

                return Json(new { success = false, message = "找不到用戶資料" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "更新失敗: " + ex.Message });
            }

        }

    }
}
