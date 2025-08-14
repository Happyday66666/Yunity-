#nullable disable

using System;
using System.Text;
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
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly IEmailSender _sender;
        public bool IsManager { get; set; }


        public RegisterConfirmationModel(UserManager<YunityUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

      
        public string Email { get; set; }

       
        public bool DisplayConfirmAccountLink { get; set; }
     
        public string EmailConfirmationUrl { get; set; }
      
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            IsManager = user != null && user.Role == "Manager";
            if (user == null)
            {
                return NotFound($"無法載入電子郵件為 '{email}' 的使用者。");
            }

            Email = email;
        
            DisplayConfirmAccountLink = false;

            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }

       

      

    }
}
