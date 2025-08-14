using System.ComponentModel.DataAnnotations.Schema;
using Yunity.Areas.Identity.Data;
using Yunity.Models;

namespace Yunity.ViewModels
{
    public class UserAreaReserveViewModel
    {
       

        // 使用者相關資訊
        public int UserId { get; set; }  // 用戶 ID
       public YunityUser? User { get; set; }  // 可選：如果需要完整的使用者資訊

        // 公設相關資訊

        public int? AreaId { get; set; } // 公設 ID

       public List<PublicAreaReserve>? PublicAreaReserves { get; set; }
       public List<UserAreaReserveWithAreaInfo>? UserAreaReserves { get; set; }

        // 門牌號碼
        public int DoorNoId { get; set; }

        // 預約狀態
        public int State { get; set; } // 0 = 未處理

     
     
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        // **修正 StartTime 和 EndTime**
       
        public DateTime? ReserveTime { get; set; }
        public int? UserPoints { get; internal set; }
        [NotMapped]  // 不需要存入資料庫
        public int? RemainingPoints { get; internal set; }

        [NotMapped]  // 不需要存入資料庫
        public int? DeductPoints { get; set; }
        //public int PeopleCount { get; internal set; }
        public int? Amount { get; internal set; }

        public int? ReservedPeople { get; set; }

        public string? ReservationDate { get; set; }  // 使用者選擇的日期（字串）
        public string? ReservationTimeSlot { get; set; } // 使用者選擇的時段（例如 "08:00~09:00"）

        // 用來顯示公設資訊
        public PublicAreaReserve? PublicArea { get; set; }
        public int? Id { get; internal set; }
        public string? OpenTime { get; internal set; }
        public string? CloseTime { get; internal set; }

        public int? IntervalHours { get; internal set; }
        public int? TotalDeduction { get; internal set; }
    }
}