#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly BuildingDataContext _buildingDataContext;

        public ConfirmEmailChangeModel(UserManager<YunityUser> userManager, SignInManager<YunityUser> signInManager, BuildingDataContext buildingDataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _buildingDataContext = buildingDataContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                StatusMessage = "Error changing email.";
                return Page();
            }

            // 更新 TusersInfo.FEmil
            var userInfo = await _buildingDataContext.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId == userId);
            if (userInfo != null)
            {
                userInfo.FEmail = email;
                _buildingDataContext.TusersInfos.Update(userInfo);
                await _buildingDataContext.SaveChangesAsync();
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = "Error changing user name.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thank you for confirming your email change.";
            return Page();
        }
    }
}
