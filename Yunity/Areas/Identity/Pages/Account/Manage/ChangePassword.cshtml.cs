#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Yunity.Areas.Identity.Data;

namespace Yunity.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<YunityUser> userManager,
            SignInManager<YunityUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
           
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "目前密碼")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "密碼長度必須至少 {2} 個字元且不超過 {1} 個字元。", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "新密碼")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "確認新密碼")]
            [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不匹配。")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法載入 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法載入 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("使用者成功變更了密碼。");
            StatusMessage = "您的密碼已成功變更。";

            return RedirectToPage();
        }
    }
}
