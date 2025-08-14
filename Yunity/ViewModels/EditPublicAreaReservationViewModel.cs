using Microsoft.AspNetCore.Mvc.Rendering;

namespace Yunity.ViewModels
{
    public class EditPublicAreaReservationViewModel
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public int DoorNoId { get; set; }
        public int UserId { get; set; }
        public DateTime ReserveTime { get; set; }
        public DateTime StartTime { get; set; }
        public int? ReservedPeople { get; set; }

        /// <summary>
        /// 預約日期 (從 StartTime 帶入)
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 使用者選擇的時段 (下拉選單)
        /// </summary>
        public string? TimePeriod { get; set; }

        /// <summary>
        /// 下拉選單選項 (可選時段)
        /// </summary>
        public IEnumerable<SelectListItem>? TimePeriodOptions { get; set; }

        /// <summary>
        /// 或另外提供所有時段資訊 (包含剩餘人數)
        /// </summary>
        public IEnumerable<TimeSlotViewModel>? TimeSlots { get; set; }

        // 公設資訊
        public PublicAreaInfo? PublicArea { get; set; }

        // 點數相關資訊
        public int? UserPoints { get; set; }
        public int? DeductPoints { get; set; }
        public int? RemainingPoints { get; set; }
        public int? State { get; internal set; }
        // 新增：總扣點數
        public int? TotalDeductPoints { get; set; }

      
    }

}
