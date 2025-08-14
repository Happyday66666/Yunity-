using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Yunity.Models;
using Yunity.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yunity.Controllers
{
    public class BD_MangeController : SuperController
    {
        public IActionResult BD_List(CKeywordViewModel key, string sort_Order)
        {
            BuildingDataContext context = new BuildingDataContext();
            string keyword = key.txtKeyword;

            // 初始化查詢
            IQueryable<BdList> query = context.BdLists;
            

            // 根據關鍵字過濾
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    p.BdName.Contains(keyword) ||
                    p.BdAddr.Contains(keyword) ||
                    p.ContractStart.ToString().Contains(keyword) ||
                    p.ContractEnd.ToString().Contains(keyword) ||
                    p.BdState.ToString().Contains(keyword)
                );
            }

            // 排序
            switch (sort_Order)
            {                
                case "BuildingName":
                    query = query.OrderBy(c => c.BdName);
                    break;
                case "ContractStart":
                    query = query.OrderBy(c => c.ContractStart);
                    break;
                case "ContractStart_desc":
                    query = query.OrderByDescending(c => c.ContractStart);
                    break;
                case "ContractEnd_desc":
                    query = query.OrderByDescending(c => c.ContractEnd);
                    break;
                case "BdState_desc":
                    query = query.OrderByDescending(c => c.BdState);
                    break;
                default:
                    query = query.OrderBy(c => c.Id); // 預設
                    break;
            }


            // 執行查詢並轉換為列表
            var datas = query.ToList();
            var list = datas.Select(t => new BdListWrap { BdList = t }).ToList();

            ViewBag.CurrentSort = sort_Order ?? "default";
            return View(list);

        }

        public IActionResult BD_Detailed(int? id)
        {
            if (id == null)
                return RedirectToAction("BD_List");

            BuildingDataContext context = new BuildingDataContext();

            var BdListdata = context.BdLists.FirstOrDefault(p => p.Id == id);
            var BdPermissiondata = context.BdPermissions.FirstOrDefault(p => p.BdId == id);
            string BdMemberdata = context.BdMembers.Count(p => p.BdId == id).ToString();
            int BdManagerdata = context.BdManagers.Count(p => p.BdId == id);

            if (BdListdata == null)
                return RedirectToAction("BD_List");

            var bdListWrap = new BdListWrap
            {

                BdList = BdListdata,
                BdPermission = BdPermissiondata,
                Usercount = BdMemberdata,
                Managercount = BdManagerdata
            };
            return View(bdListWrap);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("BD_List");

            BuildingDataContext context = new BuildingDataContext();

            var BdListdata = context.BdLists.FirstOrDefault(p => p.Id == id);
            var BdPermissiondata = context.BdPermissions.FirstOrDefault(p => p.BdId == id);

            if (BdListdata == null)
                return RedirectToAction("BD_List");

            var bdListWrap = new BdListWrap
            {

                BdList = BdListdata,
                BdPermission = BdPermissiondata
            };
            return View(bdListWrap);
        }

        [HttpPost]
        public IActionResult Edit(BdListWrap p)
        {
            BuildingDataContext context = new BuildingDataContext();
            BdList x = context.BdLists.FirstOrDefault(c => c.Id == p.Id);
            BdPermission y = context.BdPermissions.FirstOrDefault(c => c.BdId == p.Id);

            if (x != null)
            {
                x.BdName = p.BdName;
                x.BdAddr = p.BdAddr;
                x.ContractStart = p.ContractStart;
                x.ContractEnd = p.ContractEnd;
                x.FirstContact = p.FirstContact;
                x.FstContactTel = p.FstContactTel;
                x.SecondContact = p.SecondContact;
                x.SecContactTel = p.SecContactTel;
                x.HouseCount = p.HouseCount;
                x.BdState = p.BdState;

            }
            if (y != null)
            {
                y.ReceivePackage = p.ReceivePackage;
                y.SentPackage = p.SentPackage;
                y.ManageFee = p.ManageFee;
                y.Bulletin = p.Bulletin;
                y.PublicAreaReserve = p.PublicAreaReserve;
                y.NearbyStore = p.NearbyStore;
                y.CommunityService = p.CommunityService;
                y.Feedback = p.Feedback;
                y.VisitorRecord = p.VisitorRecord;
                y.Calendar = p.Calendar;

            }
            context.SaveChanges();
            return RedirectToAction("BD_Detailed", new { id = x.Id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BdListWrap p)
        {

            BuildingDataContext context = new BuildingDataContext();
            BdList bdList = new BdList
            {
                BdName = p.BdName,
                BdAddr = p.BdAddr,
                ContractStart = p.ContractStart,
                ContractEnd = p.ContractEnd,
                FirstContact = p.FirstContact,
                FstContactTel = p.FstContactTel,
                SecondContact = p.SecondContact,
                SecContactTel = p.SecContactTel,
                HouseCount = p.HouseCount,
                BdState = p.BdState
            };

            context.BdLists.Add(bdList);
            context.SaveChanges();

            var bdId = bdList.Id;

            BdPermission bdpermission = new BdPermission
            {
                BdId = bdId,
                ReceivePackage = p.ReceivePackage,
                SentPackage = p.SentPackage,
                ManageFee = p.ManageFee,
                Bulletin = p.Bulletin,
                PublicAreaReserve = p.PublicAreaReserve,
                NearbyStore = p.NearbyStore,
                CommunityService = p.CommunityService,
                Feedback = p.Feedback,
                VisitorRecord = p.VisitorRecord,
                Calendar = p.Calendar
            };

            context.BdPermissions.Add(bdpermission);
            context.SaveChanges();

            return RedirectToAction("BD_List");

        }

        public IActionResult Menber_List(int? id)
        {
            BuildingDataContext context = new BuildingDataContext();

            var BdListdata = context.BdLists.FirstOrDefault(p => p.Id == id);
            var BdMemberdataList = context.BdMembers.Where(p => p.BdId == id).ToList();

            if (BdListdata == null)
                return RedirectToAction("BD_List");
            List<BdMemberWrap> list = new List<BdMemberWrap>();

            foreach (var BdMemberdata in BdMemberdataList)
            {

                var TusersInfosdata = context.TusersInfos.FirstOrDefault(p => p.FId == BdMemberdata.UserId);

                var bdMemberWrap = new BdMemberWrap
                {
                    BdList = BdListdata,
                    BdMember = BdMemberdata,
                    TusersInfo = TusersInfosdata
                };

                list.Add(bdMemberWrap);
            }
            return View(list);
        }

        public IActionResult Manger_List(int? id)
        {
            BuildingDataContext context = new BuildingDataContext();

            var BdListdata = context.BdLists.FirstOrDefault(p => p.Id == id);
            var BdManagerdataList = context.BdManagers.Where(p => p.BdId == id).ToList();

            if (BdListdata == null)
                return RedirectToAction("BD_List");
            List<BdManagerWrap> list = new List<BdManagerWrap>();

            foreach (var BdManagerdata in BdManagerdataList)
            {

                var TManagerInfodata = context.TManagerInfos.FirstOrDefault(p => p.FId == BdManagerdata.ManagerId);

                var BdManagerWrap = new BdManagerWrap
                {
                    BdList = BdListdata,
                    BdManager = BdManagerdata,
                    TManagerInfo = TManagerInfodata
                };

                list.Add(BdManagerWrap);
            }
            return View(list);
        }
        [HttpPost]
        public IActionResult Manger_List(BdManagerWrap p)
        {
            if (p == null || p.Id == 0)
            {
                ModelState.AddModelError("", "無效的資料。");
                return View(p);
            }
            BuildingDataContext context = new BuildingDataContext();
            foreach (var item in Request.Form)
            {
                if (item.Key.StartsWith("Permissions_"))
                {
                    string item_id_string = item.Key.Substring("Permissions_".Length);
                    int item_id = -1;
                    int.TryParse(item_id_string, out item_id);
                    int item_value = -1;
                    int.TryParse(item.Value, out item_value);

                    BdManager BdManagerdata = context.BdManagers.FirstOrDefault(c => c.Id == item_id);

                    if (BdManagerdata == null)
                    {
                        continue;
                    }

                    BdManagerdata.Permissions = item_value;
                }
            }
            context.SaveChanges();

            return RedirectToAction("Manger_List");
        }
    }
}
