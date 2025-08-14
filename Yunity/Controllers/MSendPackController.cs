using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Yunity.Models;
using Yunity.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Yunity.Controllers
{
    public class MSendPackController : Controller
    {
        private readonly BuildingDataContext _context;
        public MSendPackController(BuildingDataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SendPack_List([FromBody] CKeywordViewModel key)
        {
            var keyword = key.Keyword;
            var packs = from p in _context.SendPacks
                         join d in _context.TusersInfos on p.UserId equals d.FId
                         join c in _context.TManagerInfos on p.ManagerId equals c.FId
                         into PackGroup
                         from Pack in PackGroup.DefaultIfEmpty()
                         where Pack == null || Pack.FBuildingId == 1
                         select new
                         {
                             FBuildingIdFromManager = Pack != null ? Pack.FBuildingId : (int?)null,
                             p.Id,
                             d.FName,
                             p.GetUser,
                             p.Type,
                             p.GetTel,
                             p.SendAddr,
                             p.Logistic,
                             p.PickUser,
                             p.PickTime,
                             p.PackPhoto,
                             p.PickLogisticTime,
                             p.State,
                             ManagerFName = Pack != null ? Pack.FName : null
                         };


            if (!string.IsNullOrEmpty(keyword))
            {
                List<string> type_search = new List<string>();
                if ("信件".Contains(keyword))
                {
                    type_search.Add("1");
                }
                if ("掛號".Contains(keyword))
                {
                    type_search.Add("2");
                }
                if ("一般包裹".Contains(keyword))
                {
                    type_search.Add("3");
                }
                if ("冷藏包裹".Contains(keyword))
                {
                    type_search.Add("4");
                }
                if ("冷凍包裹".Contains(keyword))
                {
                    type_search.Add("5");
                }

                List<string> state_search = new List<string>();
                if ("待處理".Contains(keyword))
                {
                    state_search.Add("0");
                }
                if ("已寄送".Contains(keyword))
                {
                    state_search.Add("1");
                }
                packs = packs.Where(p =>
                    p.FName.Contains(keyword) ||
                    p.GetUser.Contains(keyword) ||
                    type_search.Contains(p.Type.ToString()) ||
                    p.GetTel.Contains(keyword) ||
                    p.SendAddr.Contains(keyword) ||
                    p.Logistic.Contains(keyword) ||
                    p.PickUser.Contains(keyword) ||
                    p.PickTime.ToString().Contains(keyword) ||
                    p.PickLogisticTime.ToString().Contains(keyword) ||
                    state_search.Contains(p.State.ToString()) ||
                    p.ManagerFName.Contains(keyword)
                );
            }
            packs = packs.OrderByDescending(p => p.PickLogisticTime);
            return Json(packs);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SendPackWrap updatedPack)
        {
            try
            {
                if (updatedPack == null)
                    return Json(new { success = false, message = "無效資料" });

                var existingPack = _context.SendPacks.FirstOrDefault(p => p.Id == updatedPack.Id);
                if (existingPack == null)
                    return Json(new { success = false, message = "未找到對應的包裹" });
                if (updatedPack.PickUser == null)
                    updatedPack.PickUser = "";
                existingPack.GetUser = updatedPack.GetUser;
                existingPack.Type = updatedPack.Type;
                existingPack.GetTel = updatedPack.GetTel;
                existingPack.SendAddr = updatedPack.SendAddr;
                existingPack.Logistic = updatedPack.Logistic;
                existingPack.PickUser = updatedPack.PickUser;
                existingPack.PickTime = updatedPack.PickTime;
                existingPack.State = updatedPack.State;

                if (updatedPack.ImageFile != null && updatedPack.ImageFile.Length > 0)
                {
                    string PackPhoto = Guid.NewGuid().ToString() + Path.GetExtension(updatedPack.ImageFile.FileName);
                    string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SendPackPicture");

                    if (!Directory.Exists(imgPath))
                    {
                        Directory.CreateDirectory(imgPath);
                    }

                    string filePath = Path.Combine(imgPath, PackPhoto);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        updatedPack.ImageFile.CopyTo(stream);
                    }

                    existingPack.PackPhoto = PackPhoto;
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
        public IActionResult Create([FromForm] SendPackWrap CreatePack)
        {
            try
            {
                var User = _context.TusersInfos.FirstOrDefault(p => p.FAspUserId == CreatePack.FAspUserId);
                if (CreatePack == null || User == null)
                    return Json(new { success = false, message = "無效資料" });

                if (CreatePack.ImageFile == null || CreatePack.ImageFile.Length == 0)
                {
                    return Json(new { success = false, message = "圖片文件無效" });
                }

                string PackPhoto = Guid.NewGuid().ToString() + Path.GetExtension(CreatePack.ImageFile.FileName);

                if (CreatePack.ImageFile != null && CreatePack.ImageFile.Length > 0)
                {
                    string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SendPackPicture");

                    if (!Directory.Exists(imgPath))
                    {
                        Directory.CreateDirectory(imgPath);
                    }

                    string filePath = Path.Combine(imgPath, PackPhoto);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        CreatePack.ImageFile.CopyTo(stream);
                    }
                }

                var sendPack = new SendPack
                {
                    UserId = User.FId,
                    ManagerId = 1,
                    Type = CreatePack.Type,
                    GetUser = CreatePack.GetUser,
                    GetTel = CreatePack.GetTel,
                    SendAddr = CreatePack.SendAddr,
                    Logistic = CreatePack.Logistic,
                    PackPhoto = PackPhoto,
                    PickLogisticTime = DateTime.Now,
                    State = 1,
                    PickUser = "",
                };

                _context.SendPacks.Add(sendPack);
                _context.SaveChanges();

                return Json(new { success = true, message = "新增成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "資料有誤，請檢查後再提交。" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] SendPackWrap DeletePack)
        {
            try
            {
                var pack = _context.SendPacks.FirstOrDefault(p => p.Id == DeletePack.Id);

                if (pack != null)
                {
                    _context.SendPacks.Remove(pack);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "刪除成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "刪除紀錄失敗" });
            }
        }
    }
}
