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
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<YunityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required(ErrorMessage = "電子郵件為必填欄位")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
            [Display(Name = "電子郵件")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {

                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                // ✅ 發送中文化的重設密碼電子郵件
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "重設您的密碼",
                    $@"
            <p>您好，</p>
            <p>我們收到您重設密碼的請求。請點擊以下連結以重設您的密碼：</p>
            <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>👉 點擊此處重設密碼 👈</a></p>
            <p>如果您沒有提出此請求，請忽略此郵件。</p>
            <p>此郵件為系統自動發送，請勿回覆。</p>
            <p>感謝您的使用！</p>
            <p><strong>Yunity 雲社區</strong></p>"
                );

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
