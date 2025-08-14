using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class MGetPackController : Controller
    {
        private readonly BuildingDataContext _BDcontext;
        public MGetPackController(BuildingDataContext BDcontext)
        {
            _BDcontext = BDcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetPackList()
        {
            var packs = (from p in _BDcontext.GetPacks
                         join d in _BDcontext.DoorNos on p.DoorNoId equals d.Id into doorGroup
                         from door in doorGroup.DefaultIfEmpty()
                         where p.BdId == 1 //之後將數字改成登入者的BdId 
                         select new
                         {
                             p.Id,
                             p.PackNo,
                             p.Type,
                             DoorNo = door.DoorNo1,
                             p.GetUser,
                             p.Logistic,
                             p.LogTime,
                             p.State,
                             p.PickTime
                         }).OrderBy(p => p.State)
                        .ThenByDescending(p => p.LogTime)
                        .ToList();


            return Json(packs);
        }

        [HttpPost]
        public JsonResult SearchPack([FromForm] string keyword, [FromForm] int? state = null)
        {
            var packs = (from p in _BDcontext.GetPacks
                         join d in _BDcontext.DoorNos on p.DoorNoId equals d.Id into doorGroup
                         from door in doorGroup.DefaultIfEmpty()
                         where (string.IsNullOrEmpty(keyword) || door.DoorNo1.Contains(keyword) || p.GetUser.Contains(keyword) || p.PackNo.Contains(keyword)) && (p.State == state || state == null)
                         select new
                         {
                             p.Id,
                             p.PackNo,
                             p.Type,
                             DoorNo = door.DoorNo1,
                             p.GetUser,
                             p.Logistic,
                             p.LogTime,
                             p.State,
                             p.PickTime
                         }).OrderBy(p => p.State)
                        .ThenByDescending(p => p.LogTime)
                        .ToList();

            return Json(packs);
        }


        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var pack = _BDcontext.GetPacks.Where(b => b.Id == id).Select(b => new GetPackWrap
            {
                Id = b.Id,
                Type = b.Type,
                GetUser = b.GetUser,
                PickUser = b.PickUser,
                Logistic = b.Logistic,
                LogTime = b.LogTime,
                State = b.State,
                PickTime = b.PickTime,
                DoorNo = _BDcontext.DoorNos.Where(p => p.Id == b.DoorNoId).Select(p => p.DoorNo1).FirstOrDefault(),
                ImageUrl = string.IsNullOrEmpty(b.PackPhoto) ? "/images/noPic.jpeg" : "/GetPackPhoto/" + b.PackPhoto,
                Poster = _BDcontext.TManagerInfos.Where(p => p.FId == b.ManagerId).Select(p => p.FName).FirstOrDefault() ?? "未知"
            }).FirstOrDefault();

            if (pack == null)
            {
                //Console.WriteLine($"{id} not found.");
                return NotFound();
            }
            return View(pack);
        }

        public IActionResult Edit(int? id)
        {
            using (BuildingDataContext db = new BuildingDataContext())
            {
                var data = (from p in db.GetPacks
                            join d in db.DoorNos on p.DoorNoId equals d.Id into doorGroup
                            from door in doorGroup.DefaultIfEmpty()
                            where p.Id == id
                            select new GetPackWrap
                            {
                                GetPack = p,
                                DoorNo = door.DoorNo1
                            }).FirstOrDefault();

                if (data == null)
                {
                    return NotFound();
                }


                return View(data);
            }
        }

        [HttpPost]
        public IActionResult Edit(GetPackWrap b)
        {
            BuildingDataContext db = new BuildingDataContext();
            GetPack B = db.GetPacks.FirstOrDefault(c => c.Id == b.Id);

            if (B != null)
            {
                if (b.ImageFile != null && b.ImageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(b.ImageFile.FileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/GetPackPhoto");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        b.ImageFile.CopyTo(stream);
                    }

                    B.PackPhoto = fileName; 
                }
                else
                {
                    B.PackPhoto = db.GetPacks.Where(p => p.Id == b.Id).Select(p => p.PackPhoto).FirstOrDefault();
                }

                B.PickTime = b.PickTime;
                B.PackNo = b.PackNo;
                B.Type = b.Type;
                B.GetUser = b.GetUser;
                B.DoorNoId = db.DoorNos.Where(p => p.DoorNo1 == b.DoorNo).Select(p => p.Id).FirstOrDefault();
                B.Logistic = b.Logistic;
                B.PickUser = b.PickUser;
                B.State = b.State;
            }

            db.SaveChanges();
            return RedirectToAction("Detail", new { id = B.Id });

        }

        //MGetPack/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GetPackWrap G)
        {
            BuildingDataContext db = new BuildingDataContext();
            string fileName = null;

            if (G.ImageFile != null && G.ImageFile.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/GetPackPhoto");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(G.ImageFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    G.ImageFile.CopyTo(stream);
                }
            }

            var doorID = db.DoorNos.Where(p => p.DoorNo1 == G.DoorNo && p.BdId == 1).Select(p => p.Id).FirstOrDefault();  //之後搜尋條件要改成登入者的大樓ID
            if (doorID == 0)
            {
                ModelState.AddModelError("DoorNo", "找不到對應門牌號碼");
                return View(G);
            }

            GetPack B = new GetPack
            {
                PackNo = G.PackNo,
                Type = G.Type,
                DoorNoId = doorID,
                GetUser = G.GetUser,
                Logistic = G.Logistic,
                LogTime = DateTime.Now,
                PackPhoto = fileName,
                State = 0,
                ManagerId = 1, //之後改成登入者的ID
                BdId = 1       //之改成登入者的BdID
            };

            db.GetPacks.Add(B);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            try
            {
                using (BuildingDataContext db = new BuildingDataContext())
                {
                    GetPack x = db.GetPacks.FirstOrDefault(c => c.Id == id);
                    db.GetPacks.Remove(x);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(GetPackWrap GP)
        {
            var package = _BDcontext.GetPacks.FirstOrDefault(p => p.Id == GP.Id);
            if (package == null)
            {
                return Json(new { success = false, message = "包裹不存在" });
            }

            var People = _BDcontext.TusersInfos
                .FirstOrDefault(s => s.FBuildingId == 1 && s.FAspUserId == GP.PickUser);

            if (People == null)
            {
                return Json(new { success = false, message = "找不到領取人資料,請再次核對身分" });
            }

            package.State = 1;
            package.PickTime = DateTime.Now;
            package.PickUser = People.FName;

            _BDcontext.SaveChanges();

            return Json(new { success = true, message = "包裹領取成功！" });
        }


    }
}
