using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Yunity.Models;
using Yunity.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Yunity.Controllers
{
    public class MPublicAreaReserveController : Controller
    {
        private readonly BuildingDataContext _context;
        public MPublicAreaReserveController(BuildingDataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List([FromBody]CKeywordViewModel key)
        {
            var keyword = key.Keyword;
            var pa_id = key.Pa_id;
            var Reserves = from p in _context.PublicAreaReserves
                           join d in _context.UserAreaReserves on p.Id equals d.AreaId
                           join c in _context.TusersInfos on d.UserId equals c.FId
                           join a in _context.PublicAreas on p.Icon equals a.Id
                           join b in _context.TManagerInfos on p.BdId equals b.FBuildingId
                           into ReserveGroup
                           from Reserve in ReserveGroup.DefaultIfEmpty()
                           where (Reserve == null || (Reserve != null && Reserve.FId == 1)) &&
                           (pa_id == null || (pa_id != null && p.Id == pa_id))
                           select new
                           {
                               FBuildingIdFromManager = Reserve != null ? Reserve.FBuildingId : (int?)null,
                               p.Id,
                               a.PaName,
                               p.OpenTime,
                               p.CloseTime,
                               p.DeductUnit,
                               p.UseTime,
                               p.AreaInfo,
                               p.Amount,
                               d.AreaId,
                               d.StartTime,
                               d.EndTime,
                               d.State,
                               c.FName,
                               p.Photo,
                               p.Icon
                           };

            if (!string.IsNullOrEmpty(keyword))
            {
                List<string> state_search = new List<string>();
                if ("已預約".Contains(keyword)) state_search.Add("1");
                if ("使用完畢".Contains(keyword)) state_search.Add("2");
                if ("取消預約".Contains(keyword)) state_search.Add("3");

                Reserves = Reserves.Where(p =>
                    p.FName.Contains(keyword) ||
                    (p.StartTime != null && p.StartTime.ToString().Contains(keyword)) ||
                    (p.State != null && state_search.Contains(p.State.ToString())) 
                );
            }

            return Json(Reserves.OrderByDescending(p => p.EndTime));
        }

        public JsonResult PA_List()
        {
            var Reserves = from p in _context.PublicAreaReserves
                           join c in _context.PublicAreas on p.Icon equals c.Id
                           join b in _context.TManagerInfos on p.BdId equals b.FBuildingId
                           into ReserveGroup
                           from Reserve in ReserveGroup.DefaultIfEmpty()
                           where Reserve == null || (Reserve != null && Reserve.FId == 1)
                           select new
                           {
                               p.Id,
                               c.PaName,
                               p.OpenTime,
                               p.CloseTime,
                               p.DeductUnit,
                               p.UseTime,
                               p.AreaInfo,
                               p.Amount,
                               p.Photo,
                               c.Icont,
                               PubilcArea_id = p.Icon
                           };
            return Json(Reserves);
        }

        public JsonResult PA()
        {
            var reserves = _context.PublicAreas
                    .Select(p => new
                    {
                        p.Id,
                        p.PaName
                    })
                    .ToList();

            return Json(reserves);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] PublicAreaReserveWrap updatedPA)
        {
            try
            {
                if (updatedPA == null)
                    return Json(new { success = false, message = "無效資料" });

                var existingPA = _context.PublicAreaReserves.FirstOrDefault(p => p.Id == updatedPA.Id);
                if (existingPA == null)
                    return Json(new { success = false, message = "未找到對應公設資料" });

                existingPA.OpenTime = updatedPA.OpenTime;
                existingPA.CloseTime = updatedPA.CloseTime;
                existingPA.DeductUnit = updatedPA.DeductUnit;
                existingPA.UseTime = updatedPA.UseTime;
                existingPA.AreaInfo = updatedPA.AreaInfo;
                existingPA.Amount = updatedPA.Amount;
                existingPA.Icon = updatedPA.Icon;

                if (updatedPA.ImageFile != null && updatedPA.ImageFile.Length > 0)
                {
                    string PAPhoto = Guid.NewGuid().ToString() + Path.GetExtension(updatedPA.ImageFile.FileName);
                    string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/publicArea");

                    if (!Directory.Exists(imgPath))
                    {
                        Directory.CreateDirectory(imgPath);
                    }

                    string filePath = Path.Combine(imgPath, PAPhoto);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        updatedPA.ImageFile.CopyTo(stream);
                    }

                    existingPA.Photo = PAPhoto;
                }

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromForm] PublicAreaReserveWrap CreatePA)
        {
            try
            {
                var Manager = _context.TManagerInfos.FirstOrDefault(p => p.FId == 1);
                var PAname = _context.PublicAreas.FirstOrDefault(p => p.Id == CreatePA.Icon);
                if (CreatePA == null)
                    return Json(new { success = false, message = "無效資料" });

                if (CreatePA.ImageFile == null || CreatePA.ImageFile.Length == 0)
                {
                    return Json(new { success = false, message = "圖片文件無效" });
                }

                string PAPhoto = Guid.NewGuid().ToString() + Path.GetExtension(CreatePA.ImageFile.FileName);

                if (CreatePA.ImageFile != null && CreatePA.ImageFile.Length > 0)
                {
                    string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/publicArea");

                    if (!Directory.Exists(imgPath))
                    {
                        Directory.CreateDirectory(imgPath);
                    }

                    string filePath = Path.Combine(imgPath, PAPhoto);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        CreatePA.ImageFile.CopyTo(stream);
                    }
                }

                var PublicAreaReserve = new PublicAreaReserve
                {
                    BdId = Manager.FBuildingId,
                    Name = PAname.PaName,
                    OpenTime = CreatePA.OpenTime,
                    CloseTime = CreatePA.CloseTime,
                    DeductUnit = CreatePA.DeductUnit,
                    UseTime = CreatePA.UseTime,
                    AreaInfo = CreatePA.AreaInfo,
                    Amount = CreatePA.Amount,
                    Photo = PAPhoto,
                    Icon = CreatePA.Icon
                };

                _context.PublicAreaReserves.Add(PublicAreaReserve);
                _context.SaveChanges();

                return Json(new { success = true, message = "新增成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "資料有誤，請檢查後再提交。" });
            }
        }
    }
}
