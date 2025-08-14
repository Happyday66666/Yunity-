#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly UserManager<YunityUser> _userManager;
        private readonly IUserStore<YunityUser> _userStore;
        private readonly IUserEmailStore<YunityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly BuildingDataContext _buildingDataContext;

        public RegisterModel(
            UserManager<YunityUser> userManager,
            IUserStore<YunityUser> userStore,
            SignInManager<YunityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, BuildingDataContext buildingDataContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

            _buildingDataContext = buildingDataContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
 
            [Required(ErrorMessage = "請輸入您的姓名。")]
            [DataType(DataType.Text)]
            [Display(Name = "姓名")]
            public string FName { get; set; }

            [Display(Name = "地址")]
            public string FUserAddress { get; set; }
           
            [Required(ErrorMessage = "請選擇您的角色。")]
            public string Role { get; set; }

            [Phone(ErrorMessage = "請輸入有效的電話號碼。")]
            [Display(Name = "電話")]
            public string PhoneNumber { get; set; }


            [Required(ErrorMessage = "請輸入您的電子郵件地址。")]
            [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "請輸入您的密碼。")]
            [StringLength(100, ErrorMessage = "{0} 的長度必須介於 {2} 到 {1} 個字元之間。", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "密碼")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "確認密碼")]
            [Compare("Password", ErrorMessage = "密碼與確認密碼不一致。")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.PhoneNumber = Input.PhoneNumber; 
                user.Role = Input.Role;

                // 設置待審核標記
                // 住戶 & 公司帳戶可以直接使用，但管理員需要審核
                bool isApproved = Input.Role != "Manager";

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("使用者已使用密碼建立新帳戶。");

                    var userId = await _userManager.GetUserIdAsync(user);

                    // 根據角色決定是否立即發送驗證信
                    if (Input.Role != "Manager")
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // ✅ 發送中文化的電子郵件
                    await _emailSender.SendEmailAsync(Input.Email, "請確認您的電子郵件",
                        $@"
                        <p>您好，</p>
                        <p>感謝您註冊 Yunity 帳戶。請點擊以下連結以確認您的電子郵件地址：</p>
                        <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>👉 點擊此處確認您的帳戶 👈</a></p>
                        <p>如果您未註冊此帳戶，請忽略此郵件。</p>
                        <p>此郵件為系統自動發送，請勿回覆。</p>
                        <p>感謝您的使用！</p>
                        <p><strong>Yunity 雲社區</strong></p>"
                    );
                    }
                    else
                    {
                        // ❌ 管理員帳戶不發送驗證信，等待審核
                        _logger.LogInformation("管理員帳戶已建立，等待審核通過後再發送驗證信。");
                    }

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // 根據角色進行資料插入
                    if (Input.Role == "Manager")
                    {
                        var managerInfo = new TManagerInfo
                        {
                            FAspUserId= userId,
                            FName = Input.FName,
                            FAccount = Input.Email,
                            FPhone = Input.PhoneNumber,
                            FEmail = Input.Email,
                            IsApproved = "Pending" // 設定為待審核

                            // 如果有需要可以加入更多欄位
                        };

                        _buildingDataContext.TManagerInfos.Add(managerInfo);
                        await _buildingDataContext.SaveChangesAsync();
                    }
                    else if (Input.Role == "User")
                    {
                        var userInfo = new TusersInfo
                        {
                            FAspUserId = userId,
                            FName = Input.FName,
                           // FBuildingId = Input.FBuildingId,
                            FEmail = Input.Email,
                            FAccount = Input.Email,
                            FPhone = Input.PhoneNumber,
                            
                        };

                        _buildingDataContext.TusersInfos.Add(userInfo);
                        await _buildingDataContext.SaveChangesAsync();
                       

                    }
                    else if (Input.Role == "Company")
                    {
                        var companyAccount = new CompanyAccount
                        {
                            FAspUserId = userId,
                            ComName = Input.FName,
                            //FBuildingId = Input.FBuildingId,
                           ComAccount = Input.Email
                        };
                        var companyProfile = new CompanyProfile
                        {                           
                            ComEmail = Input.Email,
                            ComPhone = Input.PhoneNumber
                        };
                        _buildingDataContext.CompanyAccounts.Add(companyAccount);
                        await _buildingDataContext.SaveChangesAsync();
                        
                    }

                 if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    // ModelState.AddModelError(string.Empty, error.Description);
                    switch (error.Code)
                    {
                        //case "DuplicateUserName":
                        //    ModelState.AddModelError(string.Empty, "此帳號已被使用。");
                        //    break;
                        case "DuplicateEmail":
                            ModelState.AddModelError(string.Empty, "此電子郵件已被使用。");
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
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SendEmailVerification(YunityUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, token = token },
                protocol: Request.Scheme);

            // 中文化電子郵件標題與訊息
            var subject = "請確認您的電子郵件";
            var message = $@"
    <p>您好，</p>
    <p>感謝您註冊 <strong>Yunity</strong> 帳戶。請點擊以下連結以確認您的電子郵件地址：</p>
    <p><a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>👉 點擊此處確認您的帳戶 👈</a></p>
    <p>如果您未註冊此帳戶，請忽略此郵件。</p>
    <p>此郵件為系統自動發送，請勿回覆。</p>
    <br/>
    <p>感謝您的使用！</p>
    <p><strong>Yunity 雲社區 團隊</strong></p>";

            //var subject = "請驗證您的 Email";
            //var message = $"請點擊以下連結驗證您的 Email: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>驗證 Email</a>";

            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }


        private YunityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<YunityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(YunityUser)}'. " +
                    $"Ensure that '{nameof(YunityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<YunityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<YunityUser>)_userStore;
        }
    }
}
