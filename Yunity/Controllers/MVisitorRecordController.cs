using Microsoft.AspNetCore.Mvc;
using Yunity.Models;

namespace Yunity.Controllers
{
    public class MVisitorRecordController : Controller
    {
        private readonly BuildingDataContext _BDcontext;
        public MVisitorRecordController(BuildingDataContext BDcontext)
        {
            _BDcontext = BDcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetVisitorList()
        {
            var visitor = (from p in _BDcontext.VisitorRecords
                         join d in _BDcontext.DoorNos on p.DoorNoId equals d.Id into doorGroup
                         from door in doorGroup.DefaultIfEmpty()
                         where p.BdId == 1 //之後將數字改成登入者的BdId 
                         select new
                         {
                             p.Id,
                             p.VisitorName,
                             DoorNo = door.DoorNo1,
                             p.VisitTime,
                             p.VisitReason
                         }).OrderByDescending(p => p.VisitTime) 
                        .ToList();


            return Json(visitor);
        }

        [HttpPost]
        public JsonResult SearchVisitor([FromForm] string keyword)
        {
            var visitor = (from p in _BDcontext.VisitorRecords
                         join d in _BDcontext.DoorNos on p.DoorNoId equals d.Id into doorGroup
                         from door in doorGroup.DefaultIfEmpty()
                         where (string.IsNullOrEmpty(keyword) || door.DoorNo1.Contains(keyword) || p.VisitorName.Contains(keyword) || p.VisitReason.Contains(keyword))
                         select new
                         {
                             p.Id,
                             p.VisitorName,
                             DoorNo = door.DoorNo1,
                             p.VisitTime,
                             p.VisitReason
                         }).OrderBy(p => p.VisitTime)
                        .ToList();

            return Json(visitor);
        }

        [HttpPost]
        public JsonResult Create(VisitorRecordWrap VR)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
                );

                return Json(new { success = false, message = errors });
            }

            var doorID = _BDcontext.DoorNos.Where(p => p.DoorNo1 == VR.DoorNum && p.BdId == 1).Select(p => p.Id).FirstOrDefault();  //之後搜尋條件要改成登入者的大樓ID

            if (doorID == 0)
            {
                return Json(new { success = false, message = "找不到對應的門牌號碼：" + VR.DoorNum });
            }

            VisitorRecord V = new VisitorRecord
            {
                DoorNoId = doorID,
                VisitorName = VR.VisitorName,
                VisitReason = VR.VisitReason,
                VisitTime = DateTime.Now,
                ManagerId = 1, // 之後改成登入者 ID
                BdId = 1       // 之後改成登入者大樓 ID
            };

            _BDcontext.VisitorRecords.Add(V);
            _BDcontext.SaveChanges();

            return Json(new { success = true, message = "訪客資訊已成功提交！" });
        }





    }
}
