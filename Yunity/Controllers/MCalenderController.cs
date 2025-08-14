using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Yunity.Models;
using Calendar = Yunity.Models.Calendar;

namespace Yunity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MCalenderController : ControllerBase
    {
        private readonly BuildingDataContext _BDcontext;

        public MCalenderController(BuildingDataContext BDcontext)
        {
            _BDcontext = BDcontext;
        }

        [HttpGet("GetEventC")]
        public JsonResult GetEventC()
        {
            var CEvents = _BDcontext.Calendars
                .Where(p => p.BdId == 1)
                .Select(p => new
                {
                    id=p.Id,
                    title = p.EventName,
                    start = p.EventStart,
                    end = p.EventEnd,
                    extendedProps=new { deletable = true }
                })
                .ToList();
            return new JsonResult(CEvents);  
        }

        [HttpGet("GetEventR")]
        public JsonResult GetEventR()
        {
            //↓之後要改成登入者的大樓ID
            var datas = _BDcontext.TusersInfos.Where(p => p.FBuildingId == 1).Select(p => p.FId).ToList();

            var AEvent = _BDcontext.UserAreaReserves.Where(p =>datas.Contains((int)p.UserId) && p.State == 1).Select(p => new
            {
                id = p.Id,
                title = _BDcontext.DoorNos.Where(d => d.Id == p.DoorNoId).Select(d => d.DoorNo1).FirstOrDefault() + " 的" +
                        _BDcontext.TusersInfos.Where(t => t.FId == p.UserId).Select(t => t.FName).FirstOrDefault() + " 住戶預約 " +
                        _BDcontext.PublicAreaReserves.Where(a => a.Id == p.AreaId).Select(a => a.Name).FirstOrDefault(),
                start = p.StartTime,
                end = p.EndTime,
                extendedProps = new { deletable = false }
            });

            return new JsonResult(AEvent);
        }

        [HttpPost("AddEvent")]
        [Produces("application/json")]
        public async Task<IActionResult> AddEvent([FromBody] CalendarDTO dto)
        {
            try
            {
                var newEvent = new Calendar
                {
                    EventName = dto.Title,
                    EventStart = dto.Start,
                    EventEnd = dto.End,
                    BdId = 1,
                    ManagerId = 1,
                    LogTime = DateTime.Now,
                };

                _BDcontext.Calendars.Add(newEvent);
                await _BDcontext.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception err)
            {
                return StatusCode(500, new { success = false, message = err.Message });
            }
        }

        //刪除行事曆/取消預約
        [HttpPost("DeleteEvent")]
        public JsonResult DeleteEvent([FromBody] CalendarDTO DTO)
        {
            if (DTO == null || DTO.Id <= 0)
            {
                return new JsonResult(new { success = false, message = "無效的請求,請稍後再試" });
            }

            var Deletable = DTO.Deletable;
            if (Deletable)
            {
                return DeleteCalendarEvent(DTO.Id);
            }
            else
            {
                return CancelReservation(DTO.Id);
            }
        }

        private JsonResult DeleteCalendarEvent(int eventId)
        {
            var eventS = _BDcontext.Calendars.SingleOrDefault(p => p.Id == eventId);

            if (eventS != null)
            {
                _BDcontext.Calendars.Remove(eventS);
                _BDcontext.SaveChanges();
                return new JsonResult(new { success = true, message = "取消完成!" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "找不到事件，請稍後再試!" });
            }
        }

        private JsonResult CancelReservation(int eventId)
        {
            var eventS = _BDcontext.UserAreaReserves.SingleOrDefault(p => p.Id == eventId);

            if (eventS != null)
            {
                eventS.State = 2;  //  2 已取消
                _BDcontext.SaveChanges();
                return new JsonResult(new { success = true, message = "已取消預約!" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "找不到事件，請稍後再試!" });
            }
        }
    }


}

