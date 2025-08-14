namespace Yunity.ViewModels
{
    public class PublicAreaInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int DeductUnit { get; set; }

        /// <summary>
        /// 使用時段單位 (以小時為單位，型別為 decimal，例如 3.0 代表 3 小時)
        /// </summary>
        public decimal? UseTimeUnit { get; set; }

        // 開放及結束時間 (若資料庫儲存為 DateTime，則取其 TimeOfDay)
        public TimeOnly? Open_Time { get; set; }
        public TimeOnly? Close_Time { get; set; }

        public int? Amount { get; set; }
    }
}
