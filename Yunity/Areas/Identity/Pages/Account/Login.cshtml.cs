#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Yunity.Areas.Identity.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Yunity.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<YunityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {        
            [Required(ErrorMessage = "電子郵件為必填欄位")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
            [Display(Name = "電子郵件")]
            public string Email { get; set; }

    
            [Required(ErrorMessage = "密碼為必填欄位")]
            [DataType(DataType.Password)]
            [Display(Name = "密碼")]
            public string Password { get; set; }

            [Display(Name = "記住我")] 
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
              
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("用戶已登入");

                    // 直接從 YunityUser 取得角色
                    string Role = user.Role ?? "User";


                    // 建立 Claims，加入 fId、FName、FBuildingId、FUserAddress
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 預設 NameIdentifier
                
                 new Claim(ClaimTypes.Role, Role) // 加入角色資訊
            };

                    // 使用 SignInManager 登入時加入 Claims
                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: Input.RememberMe, claims);
 
                    // 根據角色導向不同頁面
                    switch (Role)
                    {
                        case "Company":
                            return LocalRedirect("~/VendorSide/List");
                        case "Manager":
                            return LocalRedirect("~/Concierge/Index");
                        case "User":
                        default:
                            return LocalRedirect("~/UHome/Index");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("用戶帳號已被鎖定");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "無效的登入嘗試");
                    return Page();
                }
            }

            return Page();
        }

        


    }
}
