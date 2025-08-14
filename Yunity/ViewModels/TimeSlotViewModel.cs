namespace Yunity.ViewModels
{
    /// <summary>
    /// 時段資訊 ViewModel
    /// </summary>
    public class TimeSlotViewModel
    {
        /// <summary>
        /// 時段文字，例如 "09:00~12:00"
        /// </summary>
        public string TimePeriod { get; set; }

        /// <summary>
        /// 該時段開始的完整時間
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 該時段結束的完整時間
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 剩餘可預約人數 (公設的 Amount 減去該時段已預約人數)
        /// </summary>
        public int RemainingCapacity { get; set; }
    }
}
