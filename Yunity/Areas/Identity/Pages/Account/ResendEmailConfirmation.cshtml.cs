#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Yunity.Areas.Identity.Data;

namespace Yunity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<YunityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
           
            [Required(ErrorMessage = "請輸入您的電子郵件地址。")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "驗證郵件已發送，請檢查您的電子郵件信箱。");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            // ✅ 中文化電子郵件內容
            await _emailSender.SendEmailAsync(
                Input.Email,
                "請確認您的電子郵件",
                $@"
                <p>您好，</p>
                <p>感謝您註冊 Yunity 帳戶。請點擊以下連結以確認您的電子郵件地址：</p>
                <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>點擊此處確認您的帳戶</a></p>
                <p>如果您未註冊此帳戶，請忽略此郵件。</p>
                <p>此郵件為系統自動發送，請勿回覆。</p>
                <p>感謝您的使用！</p>
                <p><strong>Yunity 團隊</strong></p>"
            );

            ModelState.AddModelError(string.Empty, "驗證郵件已發送，請檢查您的電子郵件信箱。");
            return Page();
        }
    }
}
