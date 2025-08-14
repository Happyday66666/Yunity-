namespace Yunity.ViewModels
{
    internal class PublicAreaReserveViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public int? DeductUnit { get; set; }
        public string UseTime { get; set; }
        public string AreaInfo { get; set; }
        public int? Amount { get; set; }
        public int ReservedCount { get; set; }
       // public int AvailableSpots => (int)(Amount - ReservedCount); // 計算剩餘名額
        public int AvailableSpots { get; set; } // 可預約名額
        public string? Photo { get; set; }
    }
}