using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Yunity.Models;
using Yunity.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace Yunity.Controllers
{
    public class UChatController : Controller
    {
        private readonly BuildingDataContext _context;
        public UChatController(BuildingDataContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Index([FromForm] ChatRoom user_id)
        {
            try
            {
                string msguser_id = user_id.RoomId;
                IQueryable<ChatRoom> chatRoom = _context.ChatRooms;
                if (!string.IsNullOrEmpty(msguser_id))
                {
                    chatRoom = chatRoom.Where(p => p.RoomId == msguser_id);
                }
                var datas = chatRoom.ToList();
                return PartialView("Index", datas);
            }
            catch
            {
                return PartialView();
            }
        }

        [HttpPost]
        public IActionResult Save_Msg([FromForm] ChatRoom SaveMsg)
        {
            try
            {
                var chatRoom = new ChatRoom
                {
                    RoomId = SaveMsg.RoomId,
                    UserId = SaveMsg.UserId,
                    MsgText = SaveMsg.MsgText,
                    MsgTime = DateTime.Now
                };

                _context.ChatRooms.Add(chatRoom);
                _context.SaveChanges();

                return Json(new { success = true, message = "傳送成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "傳送失敗，請稍後再試。" });
            }
        }
    }
}
