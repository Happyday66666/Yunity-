using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Data;
using Yunity.Models;
using Yunity.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yunity.Controllers
{
    public class VendorController : SuperController
    {

        public IActionResult VendorList(CKeywordViewModel vm, string sortOrder)
        {
            BuildingDataContext db = new BuildingDataContext();
            string keyword = vm.txtKeyword;
            IEnumerable<CCompanyInfoWrap> data = null;

            if (string.IsNullOrEmpty(keyword))
            {
                data = from cp in db.CompanyProfiles
                       join ca in db.CompanyAccounts on cp.ComId equals ca.Id
                       select new CCompanyInfoWrap
                       {
                           _companyPF = cp,
                           _companyAC = ca
                       };
            }
            else
            {
                data = from cp in db.CompanyProfiles
                       join ca in db.CompanyAccounts on cp.ComId equals ca.Id
                       where cp.ComServerItem.Contains(keyword)
                             || ca.ComName.Contains(keyword)
                       select new CCompanyInfoWrap
                       {
                           _companyPF = cp,
                           _companyAC = ca
                       };
            }

            //排序
            switch (sortOrder)
            {
                case "default":
                    data = data.OrderByDescending(s => s._companyAC.Id);
                    break;
                case "default_desc":
                    data = data.OrderBy(s => s._companyAC.Id);
                    break;
                case "status":
                    data = data.OrderByDescending(s => s._companyAC.ComStatus);
                    break;
                case "status_desc":
                    data = data.OrderBy(s => s._companyAC.ComStatus);
                    break;
                case "recent_end_date":
                    data = data.OrderBy(s => s._companyPF.ComContractEndDate);
                    break;
                case "recent_end_date_desc":
                    data = data.OrderByDescending(s => s._companyPF.ComContractEndDate);
                    break;
                default:
                    data = data.OrderByDescending(s => s._companyAC.Id); // 預設
                    break;
            }

            List<CCompanyInfoWrap> list = data.ToList();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_VendorListPartial", list);
            }
            return View(list);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("VendorList");

            using (BuildingDataContext db = new BuildingDataContext())
            {
                var VendorAC = db.CompanyAccounts.FirstOrDefault(c => c.Id == id);
                var VendorPF = db.CompanyProfiles.FirstOrDefault(c => c.ComId == id);
                var VendorSA = db.CompanyServiceAreas.FirstOrDefault(c => c.ComId == id);

                var data = new CCompanyWrap
                {
                    ComAR = VendorSA,
                    companyIF = VendorPF,
                    companyAC = VendorAC
                };

                return View(data);
            }
        }

        public IActionResult FixList(int? id, CKeywordViewModel vm)
        {
            if (id == null)
                return RedirectToAction("VendorList");

            using (BuildingDataContext db = new BuildingDataContext())
            {
                string keyword = vm.txtKeyword?.Trim();
                var V = db.CompanyAccounts.FirstOrDefault(v => v.Id == id);

                var data = from csar in db.CsAppointmentRecords
                           join csp in db.CsProducts on csar.ProductId equals csp.Id // CsAppointmentRecords.ProductId 等於 CsProducts.Id
                           join ca in db.CompanyAccounts on csp.ComId equals ca.Id // CsProducts.ComId 等於 CompanyAccounts.Id
                           where ca.Id == id  // 確保我們只篩選出對應的廠商
                           select new CServHistoryWrap
                           {                               
                               _CSAR = csar,
                               _CompanyAC = ca,
                               _CSP = csp
                           };
                

                // 關鍵字搜尋條件
                if (!string.IsNullOrEmpty(keyword))
                {
                    data = data.Where(d =>
                        d._CSAR.FinishDate.ToString().Contains(keyword) ||
                        d._CSP.PCategory.Contains(keyword) ||
                        d._CSAR.CustomerName.Contains(keyword) ||
                        d._CSAR.Status.Contains(keyword)
                        );
                }

                List<CServHistoryWrap> list = data.ToList();
                ViewBag.ComName = V.ComName;
                ViewBag.ID = V.Id;

                return View(list);
            }
        }


        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(CCompanyWrap C)
        {
            BuildingDataContext db = new BuildingDataContext();

            if (string.IsNullOrEmpty(C.ComName))
            {
            }
            CompanyAccount A = new CompanyAccount
            {
                ComName = C.ComName,
               // ComAccount = C.ComAccount,
                ComAccount = C.ComEmail,  // 將 ComAccount 設置為模型中的 ComEmail
                //ComPassword = C.ComPassword,
                ComPermissions = C.ComPermissions,
                ComStatus = 0
            };
            db.CompanyAccounts.Add(A);
            db.SaveChanges();

            var CID = A.Id;
            CompanyProfile P = new CompanyProfile
            {
                ComId = CID,
                ComAddress = C.ComAddress,
                ComPerson = C.ComPerson,
                ComPhone = C.ComPhone,
                ComEmail = C.ComEmail,
                ComServerItem = C.ComServerItem,
                ComRegistrationNumber = C.ComRegistrationNumber,
                ComContractStartDate = C.ComContractStartDate,
                ComContractEndDate = C.ComContractEndDate,
                ComBusinessHours = C.ComBusinessHours
            };
            db.CompanyProfiles.Add(P);
            db.SaveChanges();

            CompanyServiceArea SA = new CompanyServiceArea
            {
                ComId = CID,
                YilanC = C.YilanC,
                KeelungCity = C.KeelungCity,
                TaichungCity = C.TaichungCity,
                TainanCity = C.TainanCity,
                TaipeiCity = C.TaipeiCity,
                TaitungC = C.TaitungC,
                TaoyuanCity = C.TaoyuanCity,
                NantouC = C.NantouC,
                HsinchuC = C.HsinchuC,
                NewTaipeiCity = C.NewTaipeiCity,
                HsinchuCity = C.HsinchuCity,
                MiaoliC = C.MiaoliC,
                ChanghuaC = C.ChanghuaC,
                YunlinC = C.YunlinC,
                ChiayiC = C.ChiayiC,
                ChiayiCity = C.ChiayiCity,
                PenghuC = C.PenghuC,
                PingtungC = C.PingtungC,
                KaohsiungCity = C.KaohsiungCity,
                KinmenC = C.KinmenC,
                HualienC = C.HualienC,
                LienchiangC = C.LienchiangC
            };
            db.CompanyServiceAreas.Add(SA);
            db.SaveChanges();
            // 新增完廠商資料後，轉向手動建立使用者的頁面，
            // 並將公司資料中的 Email 自動帶入，同時指定角色為 Company
            // 轉向 Admin 控制器的 Index 頁面
            // return RedirectToAction("Index", "Admin", new { Email = C.ComEmail, role = "Company" });
            // 成功儲存後，傳遞參數到 Index 頁面並顯示 Modal

            // 顯示 Modal 的轉向
            ViewBag.ShowModal = true;
            ViewBag.Email = C.ComEmail;
            ViewBag.Role = "Company";
            // 傳遞 Email 到視圖中顯示

            return View();  // 返回同一頁面顯示 Modal
           // return RedirectToAction("Index", "Admin", new { ShowModal = true, Email = C.ComEmail, role = "Company" });
            //return RedirectToAction("VendorList");
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("VendorList");

            using (BuildingDataContext db = new BuildingDataContext())
            {
                var VendorAC = db.CompanyAccounts.FirstOrDefault(c => c.Id == id);
                var VendorPF = db.CompanyProfiles.FirstOrDefault(c => c.ComId == id);
                var VendorSA = db.CompanyServiceAreas.FirstOrDefault(c => c.ComId == id);

                var V = db.CompanyAccounts.FirstOrDefault(v => v.Id == id);

                var data = new CCompanyWrap
                {
                    ComAR = VendorSA,
                    companyIF = VendorPF,
                    companyAC = VendorAC
                };

                ViewBag.ComName = V.ComName;
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult Edit(CCompanyWrap p)
        {
            BuildingDataContext db = new BuildingDataContext();
            CompanyAccount AC = db.CompanyAccounts.FirstOrDefault(c => c.Id == p.CAid);
            CompanyProfile PF = db.CompanyProfiles.FirstOrDefault(c => c.ComId == p.CAid);
            CompanyServiceArea SA = db.CompanyServiceAreas.FirstOrDefault(c => c.ComId == p.CAid);


            if (AC != null)
            {
                AC.ComName = p.ComName;
                AC.ComPermissions = p.ComPermissions;
                AC.ComAccount = p.ComAccount;
                AC.ComStatus = p.ComStatus;
                AC.ComPassword = p.ComPassword;
            }
            if (PF != null)
            {
                PF.ComAddress = p.ComAddress;
                PF.ComPhone = p.ComPhone;
                PF.ComPerson = p.ComPerson;
                PF.ComEmail = p.ComEmail;
                PF.ComServerItem = p.ComServerItem;
                PF.ComRegistrationNumber = p.ComRegistrationNumber;
                PF.ComContractStartDate = p.ComContractStartDate;
                PF.ComContractEndDate = p.ComContractEndDate;
                PF.ComBusinessHours = p.ComBusinessHours;
            }
            if (SA != null)
            {
                SA.YilanC = p.YilanC;
                SA.KeelungCity = p.KeelungCity;
                SA.TaipeiCity = p.TaipeiCity;
                SA.NewTaipeiCity = p.NewTaipeiCity;
                SA.TaoyuanCity = p.TaoyuanCity;
                SA.HsinchuC = p.HsinchuC;
                SA.HsinchuCity = p.HsinchuCity;
                SA.MiaoliC = p.MiaoliC;
                SA.TainanCity = p.TainanCity;
                SA.TaitungC = p.TaitungC;
                SA.TaichungCity = p.TaichungCity;
                SA.ChanghuaC = p.ChanghuaC;
                SA.NantouC = p.NantouC;
                SA.YunlinC = p.YunlinC;
                SA.ChiayiC = p.ChiayiC;
                SA.ChiayiCity = p.ChiayiCity;
                SA.KaohsiungCity = p.KaohsiungCity;
                SA.PingtungC = p.PingtungC;
                SA.PenghuC = p.PenghuC;
                SA.HualienC = p.HualienC;
                SA.KinmenC = p.KinmenC;
                SA.LienchiangC = p.LienchiangC;
            }
            db.SaveChanges();
            return RedirectToAction("Details", new { id = p.CAid });
        }
    }
}