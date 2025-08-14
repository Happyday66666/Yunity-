#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using QRCoder;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<YunityUser> _userManager;
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly BuildingDataContext _buildingDataContext;


        public IndexModel(
            UserManager<YunityUser> userManager,
            SignInManager<YunityUser> signInManager, BuildingDataContext buildingDataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _buildingDataContext = buildingDataContext;
        }

        public int Points { get; set; }  //點數
        public string QRCord { get; set; } // 用來存 QR Code Base64
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Role { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "姓名")]
            public string Fname { get; set; }

            [Display(Name = "FBuildingId")]
            public int FbuildingId { get; set; }

            [Display(Name = "大樓名稱")]
            public string BuildingName { get; set; }


            [Display(Name = "地址")]
            public string FuserAddress { get; set; }

            [Display(Name = "帳號")]
            public string Account { get; set; }


            [Phone]
            [Display(Name = "電話")]
            public string PhoneNumber { get; set; }

            [Display(Name = "點數")]
            public int Points { get; set; }
        }

        private async Task LoadAsync(YunityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            string role = user.Role ?? "User";  
            string buildingName = "未提供";
            int fBuildingId = 0;
            string FuserAddress = "未提供";
            string Fname = "未提供";
            string Account = "未提供";
            int Points = 0;

            if (role == "User")
            {
                var userInfo = await _buildingDataContext.TusersInfos
                    .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
                if (userInfo != null)
                {
                    var bdmember = await _buildingDataContext.BdMembers
                        .FirstOrDefaultAsync(b => b.UserId == userInfo.FId); // 修復條件

                    if (bdmember != null)
                    {
                        var doorno = await _buildingDataContext.DoorNos
                            .FirstOrDefaultAsync(d => d.Id == bdmember.DoorNoId); // 正確應對 DoorNo

                        Points = doorno?.Points ?? 0; // 確保 Points 不為 null
                    }
                   
                    fBuildingId = userInfo.FBuildingId;
                    FuserAddress = userInfo.FUserAddress;
                    Fname = userInfo.FName;
                    Account = userInfo.FAccount;
                }
            }
            else if (role == "Company")
            {
                var companyInfo = await _buildingDataContext.CompanyAccounts
                    .FirstOrDefaultAsync(c => c.FAspUserId == user.Id);
                if (companyInfo != null)
                {
                    Fname = companyInfo.ComName ?? "未提供";
                    Account = companyInfo.ComAccount ?? "未提供";
                }

                var companyDetail = await _buildingDataContext.CompanyProfiles
                    .FirstOrDefaultAsync(c => c.ComEmail == user.Email);
                if (companyDetail != null)
                {
                    FuserAddress = companyDetail.ComAddress ?? "未提供";
                }
            }
            else if (role == "Manager")
            {
                var managerInfo = await _buildingDataContext.TManagerInfos
                    .FirstOrDefaultAsync(m => m.FAspUserId == user.Id);
                if (managerInfo != null)
                {
                    fBuildingId = managerInfo.FBuildingId;
                    Fname = managerInfo.FName ?? "未提供";
                    Account = managerInfo.FAccount ?? "未提供";
                }
            }

            if (fBuildingId != 0)
            {
                var building = await _buildingDataContext.BdLists
                    .FirstOrDefaultAsync(b => b.Id == fBuildingId);
                buildingName = building?.BdName ?? "未提供";
            }

            // 填充 InputModel
            Username = userName;
            Input = new InputModel
            {
                Role=user.Role,
                FbuildingId = fBuildingId,
                FuserAddress = FuserAddress,
                Fname = Fname,
                PhoneNumber = phoneNumber, // 使用 UserManager 取得的 PhoneNumber
                BuildingName = buildingName, 
                Account = Account,
                Points = Points
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法找到 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }
            var role = user.Role;
            if (role == "User")
            {
                var userInfo = await _buildingDataContext.TusersInfos
                    .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
          
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"無法找到 ID 為 '{_userManager.GetUserId(User)}' 的使用者。");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                Input.Role = user.Role; // 確保 Role 保持
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "更新電話號碼時發生錯誤。";
                    return RedirectToPage();
                }
            }

            // 取得使用者角色
            var roles = user.Role; 
            string role = user.Role ?? "User";  

            if (role == "User")
            {
                var userInfo = await _buildingDataContext.TusersInfos.FirstOrDefaultAsync(u => u.FAspUserId== user.Id);
                if (userInfo != null)
                {
                    userInfo.FBuildingId = Input.FbuildingId;
                    userInfo.FUserAddress = Input.FuserAddress;
                    userInfo.FName = Input.Fname;
                    userInfo.FPhone = Input.PhoneNumber;
                    userInfo.FAccount = Input.Account;

                    // 重新查詢點數，確保不會被前端修改
                    var bdmember = await _buildingDataContext.BdMembers
                        .FirstOrDefaultAsync(b => b.UserId == userInfo.FId);
                    if (bdmember != null)
                    {
                        var doorno = await _buildingDataContext.DoorNos
                            .FirstOrDefaultAsync(d => d.Id == bdmember.DoorNoId);

                        Input.Points = doorno?.Points ?? 0; // 點數來自資料庫
                    }

                    _buildingDataContext.TusersInfos.Update(userInfo);
                }
            }
            else if (role == "Manager")
            {
                var managerInfo = await _buildingDataContext.TManagerInfos.FirstOrDefaultAsync(m => m.FAspUserId == user.Id);
                if (managerInfo != null)
                {
                    managerInfo.FBuildingId = Input.FbuildingId;
                    managerInfo.FName = Input.Fname;
                    managerInfo.FPhone = Input.PhoneNumber;
                    managerInfo.FAccount = Input.Account;

                    _buildingDataContext.TManagerInfos.Update(managerInfo);
                }
            }
            else if (role == "Company")
            {
                var companyInfo = await _buildingDataContext.CompanyAccounts.FirstOrDefaultAsync(c => c.FAspUserId == user.Id);
                if (companyInfo != null)
                {                  
                    companyInfo.ComName = Input.Fname;
                    companyInfo.ComAccount = Input.Account;
                    _buildingDataContext.CompanyAccounts.Update(companyInfo);
                }
                
                var companydetail=await _buildingDataContext.CompanyProfiles.FirstOrDefaultAsync(c=>c.ComEmail==user.Email);
                if (companydetail != null) 
                {
                    companydetail.ComPhone = Input.PhoneNumber;
                    companydetail.ComAddress= Input.FuserAddress;
                    _buildingDataContext.CompanyProfiles.Update(companydetail);
                }
            }

            // 儲存變更
            await _buildingDataContext.SaveChangesAsync();

            // 重新登入，確保角色資訊不會丟失
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Role, user.Role)
    };
            await _signInManager.SignOutAsync();
            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

            //await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "您的個人資料已更新";
            
            return RedirectToPage();
        }
    }
}
