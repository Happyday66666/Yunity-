using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using Yunity.Areas.Identity.Data;
using Yunity.Data;
using Yunity.Models;
using Yunity.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using static System.Net.Mime.MediaTypeNames;

namespace Yunity.Controllers
{
    public class CommunityServiceController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;

        public CommunityServiceController(BuildingDataContext context, UserManager<YunityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1.登入的"用戶"的"大樓"有開放"社區服務"，才能進入cs_index
        // 2.用戶的大樓的”地址”要跟"廠商"有開放的"縣市"有符合至少一個，才會出現"CS_btn_01.png"的圖片
        // BuildingDataContext
        // Tusers_info：有” fBuilding_id”可以抓大樓的ID、有” fUser_address”可以抓地址
        // BD_Permissions：有”BD_ID”可以抓大樓的ID、”Community_Service”的0代表有開放”社區服務”；1代表沒開放
        // Company_Service_Area：有”COM_ID”可以抓廠商的ID，並能查看各縣市是否開放，0代表有開放；1代表沒開放

        public async Task<IActionResult> CS_Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var userInfo = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null || userInfo.FBuildingId == null)
            {
                return NotFound("無法查詢到用戶對應的大樓資訊。");
            }

            // 條件一：檢查 Community_Service 是否為 1 且對應的 BuildingId 在 BD_Permissions 中
            var bdPermission = await _context.BdPermissions.FirstOrDefaultAsync(bd => bd.BdId == userInfo.FBuildingId && bd.CommunityService == 1);
            if (bdPermission == null)
            {
                return Content("<script>alert('您的大樓未開放此功能!');window.history.back();</script>","text/html", System.Text.Encoding.UTF8);
            }

            // ********尚未成功********
            // 條件二：檢查使用者的大樓地址是否與 Company_Service_Area 符合
            // 1代表有開放

            // 取得使用者的建築物ID
            var buildingId = userInfo.FBuildingId;
            // 查詢 BDList 中對應的地址 (假設 bd_addr 儲存地址)
            var buildingInfo = await _context.BdLists.Where(bd => bd.Id == buildingId).FirstOrDefaultAsync();

            // 檢查是否找到對應的建築物地址
            if (buildingInfo != null)
            {
                // 取得建築物地址的前兩個字
                var userAdd = buildingInfo.BdAddr.Substring(0, 2);

                // 查找符合條件的 Company_Service_Area
                var companyServiceArea = await _context.CompanyServiceAreas.Where(csa =>
                    (userAdd == "宜蘭" && csa.YilanC == 1) ||
                    (userAdd == "基隆" && csa.KeelungCity == 1) ||
                    (userAdd == "台北" && csa.TaipeiCity == 1 || userAdd == "臺北" && csa.TaipeiCity == 1) ||
                    (userAdd == "新北" && csa.NewTaipeiCity == 1) ||
                    (userAdd == "桃園" && csa.TaoyuanCity == 1) ||
                    (userAdd == "新竹" && csa.HsinchuCity == 1) ||
                    (userAdd == "新竹" && csa.HsinchuC == 1) ||
                    (userAdd == "苗栗" && csa.MiaoliC == 1) ||
                    (userAdd == "台中" && csa.TaichungCity == 1 || userAdd == "臺中" && csa.TaichungCity == 1) ||
                    (userAdd == "彰化" && csa.ChanghuaC == 1) ||
                    (userAdd == "南投" && csa.NantouC == 1) ||
                    (userAdd == "雲林" && csa.YunlinC == 1) ||
                    (userAdd == "嘉義" && csa.ChiayiCity == 1) ||
                    (userAdd == "嘉義" && csa.ChiayiC == 1) ||
                    (userAdd == "台南" && csa.TainanCity == 1 || userAdd == "臺南" && csa.TainanCity == 1) ||
                    (userAdd == "高雄" && csa.KaohsiungCity == 1) ||
                    (userAdd == "屏東" && csa.PingtungC == 1) ||
                    (userAdd == "澎湖" && csa.PenghuC == 1) ||
                    (userAdd == "花蓮" && csa.HualienC == 1) ||
                    (userAdd == "台東" && csa.TaitungC == 1 || userAdd == "臺東" && csa.TaitungC == 1) ||
                    (userAdd == "金門" && csa.KinmenC == 1) ||
                    (userAdd == "連江" && csa.LienchiangC == 1)
                ).ToListAsync(); // 要寫一個一個list包起來，例如<list>

                // 檢查是否有找到符合的服務區
                if (companyServiceArea.Any())
                {
                    // 查詢每個符合的服務區是否有對應的服務類別
                    var productCategories = new List<string>();  // 用來儲存符合的服務類別
                    // 用來存儲每個產品類別對應的 ProductId
                    int? productId1 = null, productId2 = null, productId3 = null, productId4 = null;

                    foreach (var com in companyServiceArea)
                    {
                        // 查詢該服務區對應的服務類別 ****0221 修正到這裡****
                        var csProduct1 = await _context.CsProducts
                            .FirstOrDefaultAsync(cp => cp.ComId == com.ComId && cp.PCategory == "冷氣健檢");
                        if (csProduct1 != null)
                        {
                            productCategories.Add("冷氣健檢");
                            productId1 = csProduct1.Id;  // 取得對應的 ProductId
                        }

                        var csProduct2 = await _context.CsProducts
                            .FirstOrDefaultAsync(cp => cp.ComId == com.ComId && cp.PCategory == "清潔服務");
                        if (csProduct2 != null)
                        {
                            productCategories.Add("清潔服務");
                            productId2 = csProduct2.Id;  // 取得對應的 ProductId
                        }

                        var csProduct3 = await _context.CsProducts
                            .FirstOrDefaultAsync(cp => cp.ComId == com.ComId && cp.PCategory == "送洗服務");
                        if (csProduct3 != null)
                        {
                            productCategories.Add("送洗服務");
                            productId3 = csProduct3.Id;  // 取得對應的 ProductId
                        }

                        var csProduct4 = await _context.CsProducts
                            .FirstOrDefaultAsync(cp => cp.ComId == com.ComId && cp.PCategory == "電器維修");
                        if (csProduct4 != null)
                        {
                            productCategories.Add("電器維修");
                            productId4 = csProduct4.Id;  // 取得對應的 ProductId
                        }
                    }

                    // 根據服務類別列表設置 ViewBag 顯示
                    ViewBag.PCategory1 = productCategories.Contains("冷氣健檢") ? "block" : "none";
                    ViewBag.PCategory2 = productCategories.Contains("清潔服務") ? "block" : "none";
                    ViewBag.PCategory3 = productCategories.Contains("送洗服務") ? "block" : "none";
                    ViewBag.PCategory4 = productCategories.Contains("電器維修") ? "block" : "none";


                    // 設置動態的 ProductId
                    ViewBag.ProductId1 = productId1.HasValue ? productId1.Value : 0;
                    ViewBag.ProductId2 = productId2.HasValue ? productId2.Value : 0;
                    ViewBag.ProductId3 = productId3.HasValue ? productId3.Value : 0;
                    ViewBag.ProductId4 = productId4.HasValue ? productId4.Value : 0;
                }
                else
                {
                    // 如果沒有找到符合的服務區，隱藏圖片
                    ViewBag.PCategory1 = "none";
                    ViewBag.PCategory2 = "none";
                    ViewBag.PCategory3 = "none";
                    ViewBag.PCategory4 = "none";
                   
                }
                // 檢查所有產品類別是否都為 "none"
                if (ViewBag.PCategory1 == "none" && ViewBag.PCategory2 == "none" && ViewBag.PCategory3 == "none" && ViewBag.PCategory4 == "none")
                {                    
                    ViewBag.NoSever = "block";
                }
                else
                {                    
                    ViewBag.NoSever = "none";
                }

            }
            else
            {
                // 如果沒有找到建築物信息，隱藏圖片
                return Content("<script>alert('您的大樓目前尚無配合的廠商!');window.history.back();</script>", "text/html", System.Text.Encoding.UTF8);
            }


            return View();
        }

        

        public IActionResult ApptForm() //Creat功能
        {
            
            return View();
        }

        [HttpGet] // 使用 GET 方法來獲取 ProductId
        public IActionResult ApptForm(int ProductId)
        {
            // 將 ProductId 傳遞到 View 中，讓用戶填寫表單
            ViewBag.ProductId = ProductId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApptForm(CSListWrap p, int ProductId, IFormFile photo1, IFormFile photo2, IFormFile photo3) //Creat功能
        {
            var user = await 
                _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var nowUser = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == user.Id);           

            BuildingDataContext context = new BuildingDataContext();
            
            CsAppointmentRecord newAR = new CsAppointmentRecord
            {
                ProductId = ProductId, // 使用從 URL 取得的 ProductId
                UserId = nowUser.FId,  // 當前用戶ID
                CustomerName = p.CustomerName,
                Phone = p.Phone,
                ServiceLocation = p.ServiceLocation,
                Email = p.Email,
                ArNotes = string.IsNullOrEmpty(p.ArNotes) ? "客戶無填寫備註" : p.ArNotes,
                CreatedDate = DateTime.Now, // 設定當前日期時間
                Status = "已送單",// 設定預設狀態
                Feedback = "無",
                FinishDate = DateTime.MaxValue
            };
            context.CsAppointmentRecords.Add(newAR);
            context.SaveChanges();

            var CSID = newAR.Id;
            // 處理每一張照片的上傳並保存
            if (photo1 != null && photo1.Length > 0)
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string newFileName1 = $"{CSID}_{currentDate}_01{Path.GetExtension(photo1.FileName)}";

                // 照片儲存路徑
                var photoPath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "OrderPhoto", newFileName1);

                // 儲存檔案到指定路徑
                using (var stream = new FileStream(photoPath1, FileMode.Create))
                {
                    photo1.CopyTo(stream);
                }

                // 將照片資料儲存到資料庫
                CsOrderPhoto CSP = new CsOrderPhoto
                {
                    OrderId = CSID,
                    PhotoId = newFileName1 // 使用新的檔名
                };

                context.CsOrderPhotos.Add(CSP);
            }

            if (photo2 != null && photo2.Length > 0)
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string newFileName2 = $"{CSID}_{currentDate}_02{Path.GetExtension(photo2.FileName)}";
                
                var photoPath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "OrderPhoto", newFileName2);
                
                using (var stream = new FileStream(photoPath2, FileMode.Create))
                {
                    photo2.CopyTo(stream);
                }

                CsOrderPhoto CSP2 = new CsOrderPhoto
                {
                    OrderId = CSID,
                    PhotoId = newFileName2 
                };

                context.CsOrderPhotos.Add(CSP2);
            }

            // 處理 photo3
            if (photo3 != null && photo3.Length > 0)
            {

                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string newFileName3 = $"{CSID}_{currentDate}_03{Path.GetExtension(photo3.FileName)}";

                var photoPath3 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "OrderPhoto", newFileName3);

                using (var stream = new FileStream(photoPath3, FileMode.Create))
                {
                    photo3.CopyTo(stream);
                }
                CsOrderPhoto CSP3 = new CsOrderPhoto
                {
                    OrderId = CSID,
                    PhotoId = newFileName3
                };

                context.CsOrderPhotos.Add(CSP3);
            }

            context.SaveChanges();
            return RedirectToAction("CS_List");
        }


        /// CS_LIST START ////
        public async Task<IActionResult> CS_List(string sort_Order)
        {
            BuildingDataContext context = new BuildingDataContext();

            var user = await
                _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var nowUser = await _context.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (nowUser == null) // 需要確保找到了對應的用戶資料
            {
                return Content("<script>alert('您尚未有預約紀錄!');window.history.back();</script>", "text/html", System.Text.Encoding.UTF8);
            }


            var data = from x in context.CsAppointmentRecords
                       join cp in context.CsProducts on x.ProductId equals cp.Id
                       join ca in context.CompanyAccounts on cp.ComId equals ca.Id
                       where x.UserId == nowUser.FId
                       select new CSListWrap
                       {
                           Id = x.Id,
                           ComName = ca.ComName,
                           CsAppointmentRecord = x,
                           CreatedDate = x.CreatedDate,
                           Status = x.Status,
                           ProductName = cp.PName,                           
                       };

            switch (sort_Order)
            {
                case "ComName_asc":
                    data = data.OrderBy(s => s.ComName);
                    break;
                case "ComName_desc":
                    data = data.OrderByDescending(s => s.ComName);
                    break;
                case "ProductName_asc":
                    data = data.OrderBy(s => s.ProductName);
                    break;
                case "ProductName_desc":
                    data = data.OrderByDescending(s => s.ProductName);
                    break;
                case "CreatedDate_asc":
                    data = data.OrderBy(s => s.CreatedDate);
                    break;
                case "CreatedDate_desc":
                    data = data.OrderByDescending(s => s.CreatedDate);
                    break;
                case "Status_asc":
                    data = data.OrderBy(s => s.Status);
                    break;
                case "Status_desc":
                    data = data.OrderByDescending(s => s.Status);
                    break;
                default:
                    data = data.OrderByDescending(s => s.CreatedDate); // 預設
                    break;
            }

            List<CSListWrap> list = data.ToList();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_sortorderList", list);
            }
            return View(list);
        }


        /// CS_LIST END ////
        /// 





        public IActionResult CS_Detail(int? id)
        {
            BuildingDataContext context = new BuildingDataContext();

            var data = (from x in context.CsAppointmentRecords
                        join cp in context.CsProducts on x.ProductId equals cp.Id
                        join ca in context.CompanyAccounts on cp.ComId equals ca.Id
                        join cc in context.CompanyProfiles on cp.ComId equals cc.ComId
                        where x.Id == id
                        select new
                        {
                            x.Id,
                            ca.ComName,
                            x,
                            x.CreatedDate,
                            x.Status,
                            cp.PName,
                            cc.ComPhone,
                            x.CustomerName,
                            x.Phone,
                            x.ServiceLocation,
                            x.Email,
                            x.ArNotes,
                            Photos = context.CsOrderPhotos.Where(op => op.OrderId == x.Id).ToList()
                        }).AsEnumerable() // 將查詢轉換為在記憶體中執行
                        .Select(data => new CSListWrap
                        {
                            Id = data.Id,
                            ComName = data.ComName,
                            CsAppointmentRecord = data.x,
                            CreatedDate = data.CreatedDate,
                            Status = data.Status,
                            ProductName = data.PName,
                            ComPhone = data.ComPhone,
                            CustomerName = data.CustomerName,
                            Phone = data.Phone,
                            ServiceLocation = data.ServiceLocation,
                            Email = data.Email,
                            ArNotes = data.ArNotes,
                            Photo1 = data.Photos.ElementAtOrDefault(0)?.PhotoId ?? "nophoto",
                            Photo2 = data.Photos.ElementAtOrDefault(1)?.PhotoId ?? "nophoto",
                            Photo3 = data.Photos.ElementAtOrDefault(2)?.PhotoId ?? "nophoto"
                        }).FirstOrDefault();

            if (data == null)
            {
                return RedirectToAction("CS_List");
            }

            bool canCancelOrEdit = data.Status == "已送單";
            ViewBag.CanCancelOrEdit = canCancelOrEdit;

            return View(data);
        }

        [HttpPost]
        public JsonResult SubmitFeedback(int orderId, string feedback)
        {
            BuildingDataContext db = new BuildingDataContext();
            try
            {
                // 查找訂單
                var order = db.CsAppointmentRecords.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "訂單未找到。" });
                }

                // 更新反饋欄位
                order.Feedback = feedback;
                db.SaveChanges();  // 儲存變更

                return Json(new { success = true, message = "反饋已提交成功!" });  // 提供清楚的回應訊息
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "錯誤：" + ex.Message });
            }
        }

        public IActionResult CS_Edit(int? id)
        {
            BuildingDataContext context = new BuildingDataContext();

            // 如果 id 是 null，則導向列表頁面
            if (id == null)
            {
                return RedirectToAction("CS_List");
            }

           
            var data = (from x in context.CsAppointmentRecords
                        join cp in context.CsProducts on x.ProductId equals cp.Id
                        join ca in context.CompanyAccounts on cp.ComId equals ca.Id
                        join cc in context.CompanyProfiles on cp.ComId equals cc.ComId
                        where x.Id == id
                        select new
                        {
                            x.Id,
                            ca.ComName,
                            x.CreatedDate,
                            x.Status,
                            cp.PName,
                            cc.ComPhone,
                            x.CustomerName,
                            x.Phone,
                            x.ServiceLocation,
                            x.Email,
                            x.ArNotes,
                            Photos = context.CsOrderPhotos.Where(op => op.OrderId == x.Id).ToList()
                        }).AsEnumerable()
                        .Select(data => new CSListWrap
                        {
                            Id = data.Id,
                            ComName = data.ComName,
                            CreatedDate = data.CreatedDate,
                            Status = data.Status,
                            ProductName = data.PName,
                            ComPhone = data.ComPhone,
                            CustomerName = data.CustomerName,
                            Phone = data.Phone,
                            ServiceLocation = data.ServiceLocation,
                            Email = data.Email,
                            ArNotes = data.ArNotes,
                            Photo1 = data.Photos.ElementAtOrDefault(0)?.PhotoId ?? "nophoto",
                            Photo2 = data.Photos.ElementAtOrDefault(1)?.PhotoId ?? "nophoto",
                            Photo3 = data.Photos.ElementAtOrDefault(2)?.PhotoId ?? "nophoto"
                        }).FirstOrDefault();
            
            if (data == null)
            {
                return RedirectToAction("CS_List");
            }
                        
            return View(data);
            
        }

        

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CancelOrder(int orderId)
        {
            using (var context = new BuildingDataContext())
            {
                var order = context.CsAppointmentRecords.FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                {
                    return Json(new { success = false, message = "訂單未找到" });
                }

                try
                {
                    order.Status = "已取消";
                    order.FinishDate = DateTime.Now;
                    context.SaveChanges();

                    return Json(new { success = true, message = "訂單已取消" });
                }
                catch (Exception ex)
                {
                    // 如果保存資料庫時發生錯誤，回傳錯誤訊息
                    return Json(new { success = false, message = "取消訂單失敗: " + ex.Message });
                }
            }
        }


        [HttpPost]
        public IActionResult CS_Edit(CSListWrap p, IFormFile photo1, IFormFile photo2, IFormFile photo3)
        {
            BuildingDataContext context = new BuildingDataContext();

            // 查找該筆資料
            var newAR = context.CsAppointmentRecords.FirstOrDefault(x => x.Id == p.Id);
            if (newAR != null)
            {
                // 更新資料
                newAR.CustomerName = p.CustomerName;
                newAR.Phone = p.Phone;
                newAR.ServiceLocation = p.ServiceLocation;
                newAR.Email = p.Email;

                // 處理 ArNotes 的更新
                if (string.IsNullOrEmpty(p.ArNotes))
                {
                    // 若原本有備註但清空，則填寫 "客戶無填寫備註"
                    if (!string.IsNullOrEmpty(newAR.ArNotes))
                    {
                        newAR.ArNotes = "客戶無填寫備註";
                    }
                }
                else
                {                    
                    newAR.ArNotes = p.ArNotes;
                }

                context.SaveChanges(); 

                IFormFile[] photos = { photo1, photo2, photo3 };
                string[] suffixes = { "01", "02", "03" };

                bool isPhotoChanged = false;
                                
                for (int i = 0; i < photos.Length; i++)
                {
                    if (photos[i] != null && photos[i].Length > 0)
                    {
                        isPhotoChanged = UploadPhoto(context, p.Id, photos[i], suffixes[i]) || isPhotoChanged;
                    }
                }
                
                if (isPhotoChanged)
                {
                    context.SaveChanges();
                }
            }

            return RedirectToAction("CS_Detail", new { id = p.Id });
        }

        // 上傳照片
        private bool UploadPhoto(BuildingDataContext context, int orderId, IFormFile photo, string photoSuffix)
        {
            if (photo != null && photo.Length > 0)
            {
                // 刪除舊照片
                context.CsOrderPhotos.RemoveRange(context.CsOrderPhotos.Where(x => x.OrderId == orderId && x.PhotoId.Contains(photoSuffix)));

                // 處理新的照片上傳
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string newFileName = $"{orderId}_{currentDate}_{photoSuffix}{Path.GetExtension(photo.FileName)}";
                var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "OrderPhoto", newFileName);

                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                // 儲存到資料庫
                CsOrderPhoto newPhoto = new CsOrderPhoto
                {
                    OrderId = orderId,
                    PhotoId = newFileName
                };
                context.CsOrderPhotos.Add(newPhoto);
                return true; // 表示有新的照片被處理
            }

            return false; // 沒有照片處理
        }



       
    }
}

