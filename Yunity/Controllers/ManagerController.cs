using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Yunity.Partials;
using Yunity.ViewModels;
using BuildingDataContext = Yunity.Models.BuildingDataContext;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Yunity.Controllers
{
    public class ManagerController : SuperController
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly BuildingDataContext _buildingDataContext;


        public ManagerController(UserManager<YunityUser> userManager, IEmailSender emailSender, BuildingDataContext buildingDataContext)
        {
            _userManager = userManager;
            _emailSender = emailSender;

            _buildingDataContext = buildingDataContext;

        }
        // GET: Manager

        public IActionResult List(CKeywordViewModel vm, string approvalStatus)
        {
           
            BuildingDataContext db = new BuildingDataContext();
           
          
            // 讀取傳入的搜尋參數

            string keyword = vm.txtKeyword;
         
            string[] selectedFields = vm.SelectedFields;  // 將 SelectedField 改為陣列，以便支援多選
                                                          
            // 動態生成勾選框選項，從 tManager_Info 和 tBuilding 的欄位生成選項
            var fieldOptions = new List<SelectListItem>
            {
                //new SelectListItem { Text = "全部", Value = "", Selected = string.IsNullOrEmpty(field) },
            new SelectListItem { Text = "姓名", Value = "FName", Selected = selectedFields?.Contains("FName") ?? false },
            new SelectListItem { Text = "電子郵件", Value = "FEmail", Selected = selectedFields?.Contains("FEmail") ?? false },
            new SelectListItem { Text = "電話", Value = "FPhone", Selected = selectedFields?.Contains("FPhone") ?? false },
            new SelectListItem { Text = "大樓名稱", Value = "BdName", Selected = selectedFields?.Contains("BdName") ?? false }
             };


            // 將下拉選項傳遞到視圖
           ViewBag.FieldOptions = fieldOptions;

         // 初始化查詢，連接 tManager_Info 和 tBuilding
            var query = from manager in db.TManagerInfos
                        join building in db.BdLists 
                        on manager.FBuildingId equals building.Id into buildingGroup
                        from building in buildingGroup.DefaultIfEmpty() // LEFT JOIN，確保沒有大樓時仍會顯示
                        select new
                        {
                            manager.FId,
                            manager.FName,
                            manager.FPhone,
                            manager.FEmail,
                            BuildingName = manager.FBuildingId == 0 || building == null ? "無大樓" : building.BdName, // ✅ 處理無大樓的情況
                            manager.IsApproved,  // ✅ 加入 IsApproved 欄位
                        };

            // ✅ 如果選擇了審核狀態篩選
            if (!string.IsNullOrEmpty(approvalStatus))
            {
                query = query.Where(m => m.IsApproved == approvalStatus);
            }

            if (string.IsNullOrEmpty(keyword))
            {
                // 如果沒有輸入關鍵字，僅在有選中欄位時進行篩選
                if (selectedFields != null && selectedFields.Length > 0)
                {
                    query = query.Where(p => selectedFields.Contains("FName") && p.FName.Contains(keyword) ||
                                              selectedFields.Contains("FPhone") && p.FPhone.Contains(keyword) ||
                                              selectedFields.Contains("FEmail") && p.FEmail.Contains(keyword) ||
                                              selectedFields.Contains("BdName") && p.BuildingName.Contains(keyword));
                }
            }
            else
            {
                // 如果有關鍵字，檢查選中的欄位
                if (selectedFields == null || selectedFields.Length == 0)
                {
                    // 沒有選擇欄位時，搜尋所有欄位
                    query = query.Where(p => p.FName.Contains(keyword) ||
                                             p.FPhone.Contains(keyword) ||
                                             p.FEmail.Contains(keyword) ||
                                             p.BuildingName.Contains(keyword));
                }
                else
                {
                    // 如果有選中欄位，僅搜尋選中的欄位
                    query = query.Where(p => (selectedFields.Contains("FName") && p.FName.Contains(keyword)) ||
                                             (selectedFields.Contains("FPhone") && p.FPhone.Contains(keyword)) ||
                                             (selectedFields.Contains("FEmail") && p.FEmail.Contains(keyword)) ||
                                             (selectedFields.Contains("BdName") && p.BuildingName.Contains(keyword)));
                }
            }


            var datas = query.ToList().Select(p => new CManagerWrap
            {
                FId = p.FId,
                FName = p.FName,
                FPhone = p.FPhone,
                FEmail = p.FEmail,
                BuildingName = p.BuildingName,
                IsApproved = p.IsApproved  // ✅ 加入 IsApproved 資料
            }).ToList();
            
            



            // 返回篩選後的數據
            return View(datas);
            
        }

        public IActionResult Create()
        {
            // 从数据库获取建筑数据
            BuildingDataContext db = new BuildingDataContext();
            ViewBag.Buildings = new SelectList(db.BdLists.ToList(), "Id", "BdName");

            return View();
        }


        [HttpPost]
        public IActionResult Create(CManagerWrap m)
        {
            BuildingDataContext db = new BuildingDataContext();
            if (string.IsNullOrEmpty(m.FName))
            {
                ModelState.AddModelError("FName", "姓名不能为空。");
                return View(m); // 如果验证失败，返回当前视图并显示错误
            }
            
           
            // 使用传入的数据库上下文
            TManagerInfo manager = new TManagerInfo
            {
                FName = m.FName,
                FAccount = m.FAccount,
                FBuildingId = m.FBuildingId,
                
                FEmail = m.FEmail,
                FPhone = m.FPhone
            };

           

            // 将新建的经理信息添加到数据库
            db.TManagerInfos.Add(manager);
            db.SaveChanges();

            return RedirectToAction("List");
        }


        public IActionResult Detail(int? id) //edit的檢視頁面
        {
            if (id == null)
                return RedirectToAction("List");

            BuildingDataContext db = new BuildingDataContext();

            // 查詢管理員信息
            var manager = db.TManagerInfos.FirstOrDefault(t => t.FId == id);
            if (manager == null)
                return RedirectToAction("List");

            // 查詢對應的大樓名稱
            var building = db.BdLists.FirstOrDefault(b => b.Id == manager.FBuildingId);

            // 填充到 ViewModel
            var m = new CManagerWrap
            {
                FId = manager.FId,
                FName = manager.FName,
                FPhone = manager.FPhone,
                FEmail = manager.FEmail,

                BuildingName = building?.BdName ?? "無" // 若無對應大樓，設為 null



            };
            
            return View(m);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            BuildingDataContext db = new BuildingDataContext();

            // 查詢管理員信息
            TManagerInfo manager = db.TManagerInfos.FirstOrDefault(t => t.FId == id);
            if (manager == null)
                return RedirectToAction("List");

            // 查詢對應的大樓名稱
            BdList building = db.BdLists.FirstOrDefault(b => b.Id == manager.FBuildingId);
            

            // 查詢所有大樓作為下拉選項
            ViewBag.Buildings = new SelectList(db.BdLists.ToList(), "Id", "BdName", manager.FBuildingId);

            // 填充到 ViewModel
            var datas = new CManagerWrap
            {
                FId = manager.FId,
                FAspUserId = manager.FAspUserId, // ✅ 傳遞 FAspUserId
                FName = manager.FName,
                FPhone = manager.FPhone,
                FEmail = manager.FEmail,
                FBuildingId = manager.FBuildingId, // 設置當前居住大樓的 ID
                BuildingName = building?.BdName// 若無對應大樓，設為 null

            };
            
             return View(datas);
        }
        

        [HttpPost]
        public async Task<IActionResult> EditAsync(CManagerWrap m)
        {
            
            BuildingDataContext db = new BuildingDataContext();

           

            // 查詢管理員數據
            TManagerInfo dbManager = db.TManagerInfos.FirstOrDefault(p => p.FId == m.FId);
            if (dbManager != null)
            {
                // ✅ 先查詢 `user`
                var user = await _userManager.FindByIdAsync(dbManager.FAspUserId);
                if (user == null)
                {
                    Console.WriteLine($"[Error] 找不到對應的 User，FAspUserId: {dbManager.FAspUserId}");
                    return RedirectToAction("List");
                }

                Console.WriteLine($"[Debug] 找到 User，UserID: {user.Id}, FAspUserId: {dbManager.FAspUserId}");

                // ✅ 確保 user.Id 與 TManagerInfo.FAspUserId 相同
                if (user.Id != dbManager.FAspUserId)
                {
                    Console.WriteLine($"[Error] User ID 不匹配！");
                    return RedirectToAction("List");
                }

                dbManager.FName = m.FName;
                dbManager.FPhone = m.FPhone;
                dbManager.FEmail = m.FEmail;
                dbManager.FBuildingId = m.FBuildingId; // 更新大樓 ID
                dbManager.IsApproved = m.IsApproved; // ✅ 加入審核狀態的更新
                db.SaveChanges();

                // ✅ 當狀態改為 "Approved"，則執行 ApproveManager
                if (m.IsApproved == "Approved")
                {
                    Console.WriteLine($"[Debug] 正在執行 ApproveManager，UserID: {dbManager.FAspUserId}");
                    await ApproveManager(user.Id); // ✅ 改為 `user.Id`
                }
            }

            return RedirectToAction("List");
        }

        //審核管理員身分
       
        [HttpPost]
        public async Task<IActionResult> ApproveManager(string userId)
        {
            Console.WriteLine($"[Debug] ApproveManager 被執行，UserID: {userId}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("[Error] 找不到 User，可能 userId 錯誤！");
                RedirectToAction("List");
            }
            if (user != null)
            {
                // 發送 Email 驗證信
                await SendEmailVerification(user);

                // 設定為已核准
                var manager = await _buildingDataContext.TManagerInfos.FirstOrDefaultAsync(m => m.FAspUserId == userId);
                if (manager != null)
                {
                    manager.IsApproved = "Approved";
                    await _buildingDataContext.SaveChangesAsync();
                    Console.WriteLine($"[Debug] Manager {manager.FAspUserId} 已成功核准並更新 DB！");
                }
            }

            return RedirectToAction("List");
        }

        private async Task SendEmailVerification(YunityUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = token },
                protocol: Request.Scheme);

            Console.WriteLine($"[Debug] 產生驗證連結: {confirmationLink}"); // ✅ 檢查是否正確

            //var subject = "請驗證您的 Email";
            //var message = $"請點擊以下連結驗證您的 Email: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>驗證 Email</a>";
            var subject = "請確認您的管理員帳戶";
            var message = $@"
<p>您好，</p>
<p>感謝您註冊 <strong>Yunity</strong> 管理員帳戶。請點擊以下連結以驗證您的電子郵件地址：</p>
<p><a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>👉 點擊此處驗證您的帳戶 👈</a></p>
<p>如果您未註冊此帳戶，請忽略此郵件。</p>
<p>此郵件為系統自動發送，請勿回覆。</p>
<br/>
<p>感謝您的使用！</p>
<p><strong>Yunity 雲社區 管理團隊</strong></p>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, message);
                Console.WriteLine($"[Debug] Email 已發送至 {user.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Email 發送失敗：{ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectManager(int id)
        {
            var manager = await _buildingDataContext.TManagerInfos
                .FirstOrDefaultAsync(m => m.FId == id);

            if (manager != null && manager.IsApproved == "Pending")
            {
                manager.IsApproved = "Cancel"; // ✅ 設定為「取消」，保留資料
                await _buildingDataContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }

    }


}
