using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Yunity.Models;
using Yunity.ViewModels;

namespace Yunity.Controllers
{
    public class MChatController : Controller
    {
        private readonly BuildingDataContext _context;
        public MChatController(BuildingDataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Chat_room([FromBody] CKeywordViewModel key)
        {
            var keyword = key.Keyword;
            var Chat_Name = from chat in _context.ChatRooms
                            join user in _context.TusersInfos on chat.RoomId equals user.FAspUserId
                            where chat.MsgTime == (
                                from subChat in _context.ChatRooms
                                where subChat.RoomId == chat.RoomId
                                select subChat.MsgTime
                                ).Max()
                            select new
                            {
                                RoomId = chat.RoomId,
                                FName = user.FName,
                                MsgText = chat.MsgText,
                                LastMsgTime = chat.MsgTime
                            } into result
                            orderby result.LastMsgTime descending
                            select result;
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    MF_Name = MF_Name.Where(p =>
            //        p.FeeName.Contains(keyword) ||
            //        p.LogTime.ToString().Contains(keyword)
            //    );
            //}
            return Json(Chat_Name);
        }

        [HttpPost]
        public IActionResult Chat_room_Detail([FromForm] ChatRoom user_id)
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
                return PartialView("Chat_room_Detail", datas);
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
                    MangerId = SaveMsg.MangerId,
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
