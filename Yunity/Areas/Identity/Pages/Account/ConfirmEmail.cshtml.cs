#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly BuildingDataContext _db;

        public ConfirmEmailModel(UserManager<YunityUser> userManager,BuildingDataContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"無法載入 ID 為 '{userId}' 的使用者。");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "感謝您確認您的電子郵件。" : "確認電子郵件時發生錯誤。";
            // 若使用者角色為 Company，更新對應的 CompanyAccount 記錄，把 fAspUserId 設為 user.Id
            // 如果使用者的角色是 Company，則更新 Company_Account 中的 fAspUserId
            // ✅ 如果驗證成功，設定成功訊息
            if (result.Succeeded)
            {
                if (user.Role == "Company")
                {
                    // 透過 Company_Profile 的 Com_Email 來比對使用者的 Email
                    var companyProfile = _db.CompanyProfiles.FirstOrDefault(p => p.ComEmail == user.Email);
                    if (companyProfile != null)
                    {
                        // 假設 Company_Profile 的 ComId 與 Company_Account 的 Id 是相同的關聯鍵
                        var companyAccount = _db.CompanyAccounts.FirstOrDefault(a => a.Id == companyProfile.ComId);
                        if (companyAccount != null)
                        {
                            companyAccount.FAspUserId = user.Id;
                            companyAccount.ComStatus = 1;
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            else
            {
                StatusMessage = "確認電子郵件時發生錯誤。";
            }

            return Page();
        }
       
    }
}
