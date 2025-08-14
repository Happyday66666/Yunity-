using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using static System.Net.WebRequestMethods;

namespace Yunity.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        
        private readonly UserManager<YunityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly BuildingDataContext _context;

        public AdminController(ILogger<AdminController> logger, UserManager<YunityUser> userManager, IEmailSender emailSender, BuildingDataContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
            _context= context;

        }


        public IActionResult Index00(string Email, string role)
        {
            ViewBag.Email = Email;
            ViewBag.Role = role;
            // 其他邏輯...
            return View();
        }

        ////AJAX Test
        //public IActionResult Index00(string Email, string role)
        //{
        //    // 使用 ViewBag 傳遞資料
        //    ViewBag.Email = Email;
        //    ViewBag.Role = role;

        //    // 返回部分視圖，這通常用於 AJAX 載入的情況
        //    return PartialView("_SomePartialView");
        //}

        public IActionResult Index(bool ShowModal, string Email,string role)
        {
           
            ViewBag.Role = role;
            ViewBag.ShowModal = ShowModal;
            ViewBag.Email = Email;
            return View();
        }








        [HttpPost]
        public async Task<IActionResult> CreateUserManually通用(string role)
        {

            //// 1️⃣ 自動生成隨機的用戶名和電子郵件
            //string userName = GenerateRandomUserName();
            //string email = userName + "@example.com"; // 使用生成的用戶名來構建電子郵件

            // 生成唯一的 UserName
            string userName = GenerateRandomUserName();
            string email = $"{userName}@example.com";  // 生成一個唯一的電子郵件地址

            // 2️⃣ 自動生成密碼
            string password = GenerateRandomPassword();
            // 1️⃣ 建立使用者，並設定 Role 屬性
            var user = new YunityUser
            {
                UserName = userName,
                Email = email,
                Role = role,  // 直接設定 Role
                EmailConfirmed = true,
               // EmailConfirmed = false  // 初始設為未確認，等待發送確認郵件
            };

            // string password = "Admin@123";  // ⚠️ 記得使用更安全的密碼
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // 發送包含帳號、密碼和登錄鏈接的郵件
                await SendConfirmationEmail原版(user.Email, userName, password);

                // 寄送帳號和密碼
              //  await _emailSender.SendEmailAsync(email, subject, message);

                //TempData["Message"] = $"使用者建立成功，角色為 {role}！";
                // 傳遞訊息及顯示帳號密碼
                TempData["Message"] = $"使用者建立成功！帳號: {userName} 密碼: {password}";
            }
            else
            {
                TempData["Message"] = "建立失敗：" + string.Join(", ", result.Errors.Select(e => e.Description));

            }

            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> CreateUserManually(string role)
        //{
        //    // 1️⃣ 生成隨機的用戶名和電子郵件
        //    string userName = GenerateRandomUserName();
        //    string email = $"{userName}@example.com";  // 生成唯一的電子郵件

        //    // 2️⃣ 生成隨機密碼
        //    string password = GenerateRandomPassword();

        //    // 3️⃣ 創建用戶
        //    var user = new YunityUser
        //    {
        //        UserName = userName,
        //        Email = email,
        //        Role = role,  // 直接設置角色
        //        EmailConfirmed = true
        //    };

        //    // 4️⃣ 非同步創建用戶
        //    var result = await _userManager.CreateAsync(user, password);

        //    if (result.Succeeded)
        //    {
        //        // 5️⃣ 根據角色添加額外邏輯
        //        if (role == "User")
        //        {
        //            // 將資料插入 TUsersInfo 表格
        //            var tUserInfo = new TusersInfo
        //            {
        //                FName = userName,
        //                FAccount = email,

        //            };
        //            _context.TUsersInfo.Add(tUserInfo);
        //            await _context.SaveChangesAsync();
        //        }
        //        else if (role == "Manager")
        //        {
        //            // 將資料插入 TManagerInfo 表格
        //            var tManagerInfo = new TManagerInfo
        //            {
        //                UserName = userName,
        //                Email = email,
        //                ManagerCreatedAt = DateTime.UtcNow
        //            };
        //            _context.TManagerInfo.Add(tManagerInfo);
        //            await _context.SaveChangesAsync();
        //        }
        //        else if (role == "Company")
        //        {
        //            // 將資料插入 Company_Account 表格
        //            var companyAccount = new CompanyAccount
        //            {
        //                UserName = userName,
        //                Email = email,
        //                AccountCreatedAt = DateTime.UtcNow
        //            };
        //            _context.Company_Account.Add(companyAccount);
        //            await _context.SaveChangesAsync();
        //        }

        //        // 顯示成功訊息
        //        TempData["Message"] = $"使用者建立成功！帳號: {userName} 密碼: {password}";
        //    }
        //    else
        //    {
        //        // 處理創建失敗的情況
        //        TempData["Message"] = "建立失敗：" + string.Join(", ", result.Errors.Select(e => e.Description));
        //    }

        //    return RedirectToAction("Index");
        //}





        //        private string GenerateRandomUserName()
        //{
        //    const int length = 8; // 用戶名長度
        //    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        //    StringBuilder result = new StringBuilder(length);

        //    using (var rng = new RNGCryptoServiceProvider())  // 使用 RNGCryptoServiceProvider
        //    {
        //        byte[] randomBytes = new byte[length];
        //        rng.GetBytes(randomBytes);  // 生成隨機字節

        //        for (int i = 0; i < length; i++)
        //        {
        //            // 這樣做將隨機字節映射到 validChars 中的字符
        //            result.Append(validChars[randomBytes[i] % validChars.Length]);
        //        }
        //    }

        //    return result.ToString();
        //}

        //使用廠商Email
       


        private string GenerateRandomUserName()
        {
            const int length = 8; // 用戶名長度
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder result = new StringBuilder(length);

            using (var rng = RandomNumberGenerator.Create())  // 使用 RandomNumberGenerator.Create() 替代過時的 RNGCryptoServiceProvider
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);  // 生成隨機字節

                for (int i = 0; i < length; i++)
                {
                    // 這樣做將隨機字節映射到 validChars 中的字符
                    result.Append(validChars[randomBytes[i] % validChars.Length]);
                }
            }

            return result.ToString();
        }



        //private string GenerateRandomPassword()
        //{
        //    const int length = 12; // 密碼長度
        //    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        //    StringBuilder result = new StringBuilder(length);

        //    using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) // 使用新的方式來創建 RandomNumberGenerator
        //    {
        //        byte[] randomBytes = new byte[length];
        //        rng.GetBytes(randomBytes);

        //        for (int i = 0; i < length; i++)
        //        {
        //            result.Append(validChars[randomBytes[i] % validChars.Length]);
        //        }
        //    }

        //    return result.ToString();
        //}
        //private string GenerateRandomPassword()
        //{
        //    const int length = 12; // 密碼長度
        //    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        //    StringBuilder result = new StringBuilder(length);
        //    bool hasDigit = false;  // 用來追蹤是否包含數字

        //    // 生成隨機密碼
        //    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())  // 使用新的 RNG
        //    {
        //        byte[] randomBytes = new byte[length];
        //        rng.GetBytes(randomBytes);

        //        for (int i = 0; i < length; i++)
        //        {
        //            char randomChar = validChars[randomBytes[i] % validChars.Length];
        //            result.Append(randomChar);

        //            // 檢查是否包含數字
        //            if (char.IsDigit(randomChar))
        //            {
        //                hasDigit = true;
        //            }
        //        }
        //    }

        //    // 如果生成的密碼中沒有數字，則強制添加一個
        //    if (!hasDigit)
        //    {
        //        // 確保最後一個字符是數字
        //        result[result.Length - 1] = '1';  // 在密碼最後添加一個數字（可以根據需求修改）
        //    }

        //    return result.ToString();
        //}
        private string GenerateRandomPassword()
        {
            const int length = 12; // 密碼長度
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            const string nonAlphanumericChars = "!@#$%^&*()"; // 非字母數字字符集合
            StringBuilder result = new StringBuilder(length);
            bool hasDigit = false;  // 用來追蹤是否包含數字
            bool hasNonAlphanumeric = false;  // 用來追蹤是否包含非字母數字字符

            // 生成隨機密碼
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())  // 使用新的 RNG
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                {
                    char randomChar = validChars[randomBytes[i] % validChars.Length];
                    result.Append(randomChar);

                    // 檢查是否包含數字
                    if (char.IsDigit(randomChar))
                    {
                        hasDigit = true;
                    }

                    // 檢查是否包含非字母數字字符
                    if (nonAlphanumericChars.Contains(randomChar))
                    {
                        hasNonAlphanumeric = true;
                    }
                }
            }

            // 如果生成的密碼中沒有數字，則強制添加一個
            if (!hasDigit)
            {
                result[result.Length - 1] = '1';  // 在密碼最後添加一個數字（可以根據需求修改）
            }

            // 如果生成的密碼中沒有非字母數字字符，則強制添加一個
            if (!hasNonAlphanumeric)
            {
                result[result.Length - 1] = '!';  // 在密碼最後添加一個非字母數字字符
            }

            return result.ToString();
        }

        private async Task SendConfirmationEmail原版(string email, string userName, string password)
        {
            var subject = "您的帳號創建成功";

            var loginUrl = "https://localhost:7091/Identity/Account/Login";


            var message = $@"
        您的帳號已創建成功！<br>
        帳號: {userName} <br>
        密碼: {password} <br><br>
        您可以點擊以下鏈接來登錄系統：<br>
        <a href='{loginUrl}'>立即登錄</a> <br><br>
        如果您沒有請求此操作，請忽略此郵件。
    ";

            await _emailSender.SendEmailAsync(email, subject, message);
        }


       


        [HttpPost]
        public async Task<IActionResult> CreateUserManually(string role, string Email)
        {
            // 根據 email 查詢相關公司資料，必須存在且僅一筆
            var companies = await _context.CompanyProfiles
                                          .Where(c => c.ComEmail == Email)
                                          .ToListAsync();
            if (companies.Count != 1)
            {
                TempData["Message"] = "該電子郵件對應的公司資料不存在或存在多筆，請確認後再試。";
                return RedirectToAction("Index");
            }

            // 檢查使用者是否已經使用此 Email
            var normalizedEmail = _userManager.NormalizeEmail(Email);
            var existingUsers = await _userManager.Users
                                                  .Where(u => u.NormalizedEmail == normalizedEmail)
                                                  .ToListAsync();
            if (existingUsers.Count > 0)
            {
                TempData["Message"] = "該電子郵件已存在，請使用其他 Email。";
                return RedirectToAction("Index");
            }

            // 生成唯一的 UserName (此處直接使用 Email 當作使用者名稱)
            string userName = Email;
            string email = Email;

            // 自動生成密碼
            string password = GenerateRandomPassword();

            // 建立使用者，並設定 Role 屬性；EmailConfirmed 初始設為 false，等待用戶通過郵件確認
            var user = new YunityUser
            {
                UserName = userName,
                Email = email,
                Role = role,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // 取得使用者 ID
                var userId = await _userManager.GetUserIdAsync(user);

                // 生成確認 token 並進行 URL 安全編碼
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // 設定回傳網址（可依需求修改）
                string returnUrl = Url.Content("~/");

                // 使用 Url.Page 生成確認連結，指向 Identity Area 下的 ConfirmEmail 頁面
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = encodedToken, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                // 將 callbackUrl 進行 HTML 編碼
                var encodedCallbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

                // 發送包含確認連結的郵件
                await SendConfirmationEmail(user.Email, userName, password,
                    encodedCallbackUrl);

                //TempData["Message"] = $"使用者建立成功！帳號: {userName} 密碼: {password}";
                TempData["Message"] = $"廠商建立成功！<br>帳號: {userName}<br>密碼: {password}";


            }
            else
            {
                TempData["Message"] = "建立失敗：" + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            // return RedirectToAction("Index");
            // 回到廠商列表頁面
            //return RedirectToAction("VendorList");
            return RedirectToAction("VendorList", "Vendor");
        }


        // 內部發送確認郵件方法，無需標記 HTTP 屬性
        private async Task SendConfirmationEmail(string email, string userName, string password, string callbackUrl)
        {
            var subject = "請確認您的電子郵件";

            var message = $@"
<p>您好，</p>
<p>您的帳號已創建成功！<br>
帳號: {userName}<br>
密碼: {password}</p>
<p>請點擊以下連結來確認您的電子郵件地址（連結在2小時內有效）：<br>
<a href='{callbackUrl}'>👉 點擊此處確認您的帳戶 👈</a></p>
<p>如果您沒有請求此操作，請忽略此郵件。</p>
<p>此郵件為系統自動發送，請勿回覆。</p>
<p>感謝您的使用！</p>
<p><strong>Yunity 雲社區</strong></p>
";

            try
            {
                await _emailSender.SendEmailAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送確認郵件時發生錯誤");
                throw;
            }
        }


       

    }

}
