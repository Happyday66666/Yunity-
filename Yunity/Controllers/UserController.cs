using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class UserController : SuperController
    {
        public IActionResult List(CSearchFieldViewModel SF, CKeywordViewModel vm)
        {
            BuildingDataContext bd = new BuildingDataContext();

            var query = bd.TusersInfos
                .Join(bd.BdMembers,
                    user => user.FId,
                    member => member.UserId,
                    (user, member) => new { user, member })
                .Join(bd.BdLists,
                    joined => joined.member.BdId,
                    list => list.Id,
                    (joined, list) => new { joined.user, joined.member, list })
                .GroupBy(
                    item => new { item.user.FId, item.user.FName, item.user.FPhone, item.user.FBuildingId, item.user.FUserAddress, item.user.FEmail },
                    item => item.list.BdName
                )
                .Select(group => new CUsersInfoWrap
                {
                    FId = group.Key.FId,
                    FName = group.Key.FName,
                    FPhone = group.Key.FPhone,
                    FBuildingId = group.Key.FBuildingId,
                    FUserAddress = group.Key.FUserAddress,
                    FEmail = group.Key.FEmail,
                    // 這裡將所有大樓名稱放到 BuildingNames 中，並使用 Distinct 來確保不重複
                    BuildingNames = group.Distinct().ToList()
                });

            // 先從資料庫中取出所有結果
            var datas = query.ToList();

            // 搜尋邏輯
            if (!string.IsNullOrEmpty(vm.Keyword) && !string.IsNullOrEmpty(SF.SearchField))
            {
                string keyword = vm.Keyword;
                string searchField = SF.SearchField;

                if (!string.IsNullOrEmpty(keyword))
                {
                    switch (searchField)
                    {
                        case "FName":
                            datas = datas.Where(p => p.FName.Contains(keyword)).ToList();
                            break;
                        case "FPhone":
                            datas = datas.Where(p => p.FPhone.Contains(keyword)).ToList();
                            break;
                        case "FUser_address":
                            datas = datas.Where(p => p.FUserAddress.Contains(keyword)).ToList();
                            break;
                        case "FEmail":
                            datas = datas.Where(p => p.FEmail.Contains(keyword)).ToList();
                            break;
                        case "BuildingName":
                            // 在內存中對 BuildingNames 進行過濾
                            datas = datas.Where(p => p.BuildingNames.Any(b => b.Contains(keyword))).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            if (datas == null || !datas.Any())
            {
                ViewData["Message"] = "沒有找到資料";
            }

            return View(datas); // 返回的是 IEnumerable<CUsersInfoWrap>
        }
        public ActionResult Details(int? id)
        {
            
            BuildingDataContext db = new BuildingDataContext();

            // 查找用戶基本資料
            var user = db.TusersInfos.FirstOrDefault(u => u.FId == id);
            if (user == null)
            {
                return RedirectToAction("List", "User");
            }

            // 查找該用戶所屬的所有大樓名稱
            var buildings = db.BdMembers
                              .Where(bm => bm.UserId == user.FId)
                              .Join(db.BdLists,
                                    bm => bm.BdId,
                                    bl => bl.Id,
                                    (bm, bl) => bl.BdName)  // 只獲取大樓名稱
                              .ToList();

            // 構建 CUsersInfoWrap 並包含用戶資料及所有大樓名稱
            var userWrap = new CUsersInfoWrap
            {
                FId = user.FId,
                FName = user.FName,
                FPhone = user.FPhone,
                FUserAddress = user.FUserAddress,
                FEmail = user.FEmail,
                // 將所有大樓名稱放入 BuildingNames
                BuildingNames = buildings
            };

            // 返回視圖並傳遞用戶資料
            return View(userWrap);
        }
        public IActionResult Edit(int? id)
        {
             

            BuildingDataContext db = new BuildingDataContext();
            var user = db.TusersInfos.FirstOrDefault(t => t.FId == id);
            if (user == null)
                return RedirectToAction("Details");

            // 將用戶資料傳遞給視圖
            var userWrap = new CUsersInfoWrap
            {
                FId = user.FId,
                FName = user.FName,
                FPhone = user.FPhone,
                FUserAddress = user.FUserAddress,
                FEmail = user.FEmail
            };

            return View(userWrap);
        }
        [HttpPost]
        public IActionResult Edit(CUsersInfoWrap p)
        {
           

            BuildingDataContext db = new BuildingDataContext();
            var user = db.TusersInfos.FirstOrDefault(c => c.FId == p.FId);

            if (user != null)
            {
                bool isUpdated = false;

                // 更新用戶基本資料
                if (user.FName != p.FName)
                {
                    user.FName = p.FName;
                    isUpdated = true;
                }
                if (user.FPhone != p.FPhone)
                {
                    user.FPhone = p.FPhone;
                    isUpdated = true;
                }
                if (user.FUserAddress != p.FUserAddress)
                {
                    user.FUserAddress = p.FUserAddress;
                    isUpdated = true;
                }
                if (user.FEmail != p.FEmail)
                {
                    user.FEmail = p.FEmail;
                    isUpdated = true;
                }

                // 如果有更新的話，保存更改
                if (isUpdated)
                {
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Details", new { id = p.FId });
        }
    }
}