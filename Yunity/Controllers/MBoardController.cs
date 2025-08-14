using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Linq;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class MBoardController : Controller
    {
        private readonly BuildingDataContext _BDcontext;
        public MBoardController(BuildingDataContext BDcontext)
        {
            _BDcontext = BDcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBoardList()
        {
            var boards = _BDcontext.Boards.OrderByDescending(b => b.OpenTime).Select(b => new
            {
                b.Id,
                b.Type,
                b.State,
                b.Name,
                Time = b.OpenTime,
                Poster = _BDcontext.TManagerInfos.Where(p => p.FId == b.ManagerId).Select(p => p.FName).FirstOrDefault() ?? "未知"
            }).ToList();

            return Json(boards);
        }

        [HttpPost]
        public JsonResult SearchBoard([FromForm] string keyword, [FromForm] int? state = null)
        {
            var datas = _BDcontext.Boards.Where(b => (string.IsNullOrEmpty(keyword) || b.Name.Contains(keyword) || b.Info.Contains(keyword))&&(b.State == state || state == null)).OrderByDescending(b => b.OpenTime).Select(a => new
            {
                a.Id,
                a.Type,
                a.Name,
                a.State,
                Time = a.OpenTime,
                Poster = _BDcontext.TManagerInfos.Where(p => p.FId == a.ManagerId).Select(p => p.FName).FirstOrDefault() ?? "未知"
            }).ToList();

            return Json(datas);
        }


        //GET:MBoard/Detail/id
        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var board = _BDcontext.Boards.Where(b => b.Id == id).Select(b => new CBoardDetailViewModel
            {
                Id = b.Id,
                Type = b.Type.ToString(),
                State = b.State.ToString(),
                Title = b.Name,
                Time = b.OpenTime,
                Description = b.Info,
                ImageUrl = string.IsNullOrEmpty(b.Photo) ? "/images/billboard.svg" : "/boardPhoto/" + b.Photo,
                Poster = _BDcontext.TManagerInfos.Where(p => p.FId == b.ManagerId).Select(p => p.FName).FirstOrDefault() ?? "未知"
            }).FirstOrDefault();

            if (board == null)
            {
                //Console.WriteLine($"{id} not found.");
                return NotFound();
            }
            return View(board);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            using (BuildingDataContext db = new BuildingDataContext())
            {
                var board = db.Boards.FirstOrDefault(c => c.Id == id);

                var data = new BoardWrap
                {
                    Board = board
                };

                return View(data);
            }

        }

        [HttpPost]
        public IActionResult Edit(BoardWrap b, IFormFile photo, string action)
        {
            BuildingDataContext db = new BuildingDataContext();
            Board B = db.Boards.FirstOrDefault(c => c.Id == b.Id);
            int state = 0;

            if (B != null)
            {
                if (photo != null)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "boardPhoto");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var s = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(s);
                    }

                    B.Photo = fileName;
                }

                if (action == "edit")
                {
                    state = 1;
                }
                else if (action == "post")
                {
                    state = 3;
                }


                // ManagerId=登入者ID ---待登入完成更新 
                B.Name = b.Name;
                B.Info = b.Info;
                B.OpenTime = DateTime.Now;
                B.Type = b.Type;
                B.State = state;
                B.ManagerId = 1;
            }

            db.SaveChanges();
            return RedirectToAction("Detail", new { id = B.Id });
        }

        public IActionResult Create() 
        { 
            return View(); 
        }

        [HttpPost]
        public IActionResult Create(BoardWrap b,string action)
        {
            BuildingDataContext db = new BuildingDataContext();
            string fileName = null;
            int state = 0;

            if (b.ImageFile != null && b.ImageFile.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/boardPhoto");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(b.ImageFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    b.ImageFile.CopyTo(stream);
                }
            }

            if(action == "draft")
            {
                state = 1;
            }
            else if(action == "Create")
            {
                state = 3; 
            }

            Board B = new Board
            {
                Name = b.Name,
                Info = b.Info,
                Type = b.Type,
                OpenTime = DateTime.Now,
                ManagerId = 1, // 之後要改成登入者 ID
                BdId = 1,      // 之後要改成登入者大樓的ID
                Photo = fileName,
                State = state
            };

            db.Boards.Add(B);
            db.SaveChanges();

            return RedirectToAction("Detail", new { id = B.Id });
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("無效的 ID");
            }
            try
            {
                using (BuildingDataContext db = new BuildingDataContext())
                {
                    Board x = db.Boards.FirstOrDefault(c => c.Id == id);
                    if (x != null)
                    {
                        if (x.State == 1)               // 1)草稿 2)撤銷 3)發佈
                        {
                            db.Boards.Remove(x);
                        }
                        else if (x.State == 3)  
                        {
                            x.State = 2; 
                            db.Boards.Update(x);  
                        }
                        db.SaveChanges(); 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("刪除錯誤" + ex.Message);
            }

            return RedirectToAction("Index");
        }




    }
}

