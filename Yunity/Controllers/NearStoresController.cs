using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Yunity.Models;

namespace Yunity.Controllers
{
    public class NearStoresController : Controller
    {
        private readonly BuildingDataContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        

        public NearStoresController(BuildingDataContext context, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        // GET: NearStores
        public async Task<IActionResult> Index()
        {
            return View(await _context.NearStores.ToListAsync());
        }

        // GET: NearStores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nearStore = await _context.NearStores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nearStore == null)
            {
                return NotFound();
            }

            return View(nearStore);
        }

        // GET: NearStores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NearStores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BdId,Name,OpenTime,Addr,Type,Photo,Info,UpdateTime")] NearStore nearStore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nearStore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nearStore);
        }

        // GET: NearStores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nearStore = await _context.NearStores.FindAsync(id);
            if (nearStore == null)
            {
                return NotFound();
            }
            return View(nearStore);
        }

        // POST: NearStores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,BdId,Name,OpenTime,Addr,Type,Photo,Info,UpdateTime")] NearStore nearStore)
        //{
        //    if (id != nearStore.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(nearStore);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NearStoreExists(nearStore.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(nearStore);
        //}

        //修改版
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BdId,Name,OpenTime,Addr,Type,Photo,Info")] NearStore nearStore, IFormFile uploadedPhoto)
        {
            if (id != nearStore.Id)
            {
                return NotFound();
            }

            // 處理照片上傳，如果有上傳檔案的話
            if (uploadedPhoto != null && uploadedPhoto.Length > 0)
            {
                // 建立一個唯一檔名(可依需求修改)           
                var fileName = Guid.NewGuid().ToString().Substring(0, 8) + Path.GetExtension(uploadedPhoto.FileName);
                // 指定儲存路徑，例如 wwwroot/uploads 資料夾
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "NearStore");
                // 如果資料夾不存在則建立
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, fileName);

                // 儲存檔案
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedPhoto.CopyToAsync(stream);
                }
                //// 設定照片路徑（此處路徑依專案需求調整）
                //nearStore.Photo = "/NearStore/" + fileName;
                // 設定照片檔名（僅儲存檔名，不含路徑）
                nearStore.Photo = fileName;
            }

            // 後台自動更新時間，避免使用者自行編輯
            nearStore.UpdateTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nearStore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NearStoreExists(nearStore.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nearStore);
        }



        // GET: NearStores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nearStore = await _context.NearStores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nearStore == null)
            {
                return NotFound();
            }

            return View(nearStore);
        }

        // POST: NearStores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nearStore = await _context.NearStores.FindAsync(id);
            if (nearStore != null)
            {
                _context.NearStores.Remove(nearStore);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NearStoreExists(int id)
        {
            return _context.NearStores.Any(e => e.Id == id);
        }
    }
}
