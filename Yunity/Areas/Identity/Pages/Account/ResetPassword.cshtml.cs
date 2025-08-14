#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Yunity.Areas.Identity.Data;

namespace Yunity.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;

        public ResetPasswordModel(UserManager<YunityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            
            [Required(ErrorMessage = "請輸入您的電子郵件地址。")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
            public string Email { get; set; }

            [Required(ErrorMessage = "請輸入您的新密碼。")]
            [StringLength(100, ErrorMessage = "{0} 的長度必須介於 {2} 到 {1} 個字元之間。", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "確認密碼")]
            [Compare("Password", ErrorMessage = "密碼與確認密碼不一致。")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "請提供密碼重設代碼。")]
            public string Code { get; set; }

        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("密碼重設需要提供驗證代碼。");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
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
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                
                switch (error.Code)
                {
                    case "InvalidToken":
                        ModelState.AddModelError(string.Empty, "無效的重設密碼代碼，請重新請求重設密碼。");
                        break;
                    case "PasswordTooShort":
                        ModelState.AddModelError(string.Empty, "密碼過短，請輸入至少 6 個字元。");
                        break;
                    case "PasswordRequiresDigit":
                        ModelState.AddModelError(string.Empty, "密碼必須包含至少一個數字 (0-9)。");
                        break;
                    case "PasswordRequiresUpper":
                        ModelState.AddModelError(string.Empty, "密碼必須包含至少一個大寫字母 (A-Z)。");
                        break;
                    case "PasswordRequiresLower":
                        ModelState.AddModelError(string.Empty, "密碼必須包含至少一個小寫字母 (a-z)。");
                        break;
                    case "PasswordRequiresNonAlphanumeric":
                        ModelState.AddModelError(string.Empty, "密碼必須包含至少一個特殊字元 (例如: @, #, $, %)。");
                        break;
                    default:
                        ModelState.AddModelError(string.Empty, error.Description);
                        break;
                }
            }
            return Page();
        }
    }
}
