#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly BuildingDataContext _buildingDataContext;

        public EmailModel(
            UserManager<YunityUser> userManager,
            SignInManager<YunityUser> signInManager,
            IEmailSender emailSender, BuildingDataContext buildingDataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _buildingDataContext = buildingDataContext;
        }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
 
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(YunityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法載入 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法載入 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);

                // 若需要，同步更新其他資料表中的電子郵件資訊

                // 更新 TusersInfos 資料表
                var userInfo = await _buildingDataContext.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
                if (userInfo != null)
                {
                    userInfo.FEmail = Input.NewEmail;
                    userInfo.FAccount=Input.NewEmail;
                    _buildingDataContext.TusersInfos.Update(userInfo);
                }

                // 更新 TManagerInfos 資料表
                var managerInfo = await _buildingDataContext.TManagerInfos.FirstOrDefaultAsync(m => m.FAspUserId == user.Id);
                if (managerInfo != null)
                {
                    managerInfo.FEmail = Input.NewEmail;
                    managerInfo.FAccount = Input.NewEmail;
                    _buildingDataContext.TManagerInfos.Update(managerInfo);
                }

               // 更新 CompanyProfiles 資料表
                // 此處以原來的電子郵件為查詢條件
                var companyProfile = await _buildingDataContext.CompanyProfiles.FirstOrDefaultAsync(c => c.ComEmail == Email);
                if (companyProfile != null)
                {
                    companyProfile.ComEmail = Input.NewEmail;
                    _buildingDataContext.CompanyProfiles.Update(companyProfile);
                }

                var companyAccount= await _buildingDataContext.CompanyAccounts.FirstOrDefaultAsync(a=>a.ComAccount == Email);
                if (companyAccount != null)
                {
                    companyAccount.ComAccount = Input.NewEmail;
                    _buildingDataContext.CompanyAccounts.Update(companyAccount);
                }

                // 儲存所有資料表的更新
                await _buildingDataContext.SaveChangesAsync();



                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);

                // ✅ 修改這裡的電子郵件內容
                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "請確認您的電子郵件",
                    $@"
            <p>您好，</p>
            <p>您已請求更改您的電子郵件地址。請點擊以下連結以確認您的電子郵件：</p>
            <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>點擊此處確認您的帳戶</a></p>
            <p>如果您未請求此更改，請忽略此郵件。</p>
            <p>此郵件為系統自動發送，請勿回覆。</p>
            <p>感謝您的使用！</p>
            <p><strong>Yunity 雲社區</strong></p>"
                );              

                StatusMessage = "已發送電子郵件更改的確認連結。請檢查您的電子郵件。";
                return RedirectToPage();
            }

            StatusMessage = "您的電子郵件未更改。";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法載入 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            // ✅ 修改這裡的電子郵件內容
            await _emailSender.SendEmailAsync(
                email,
                "請驗證您的電子郵件",
                $@"
        <p>您好，</p>
        <p>請點擊以下連結以驗證您的電子郵件並完成帳戶設置：</p>
        <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>點擊此處驗證您的電子郵件</a></p>
        <p>如果您未請求此操作，請忽略此郵件。</p>
        <p>此郵件為系統自動發送，請勿回覆。</p>
        <p>感謝您的使用！</p>
        <p><strong>Yunity 雲社區</strong></p>"
            );

            StatusMessage = "已發送驗證電子郵件。請檢查您的電子郵件。";
            return RedirectToPage();
        }
    }
}
