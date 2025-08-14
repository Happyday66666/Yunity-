namespace Yunity.ViewModels
{
    public class ManageFeeViewModel
    {
        public List<int> SelectedFees { get; set; }
        public int Id { get; set; }
        public string FeeName { get; set; }
        public int? Price { get; set; }
        public int? MotorPrice { get; set; }
        public int? CarPrice { get; set; }
        public DateTime? FeeEnd { get; set; }
        public int? PayType { get; set; }
        public DateTime? PayTime { get; set; }
        public int? State { get; set; }

        // 顯示付款方式，如果未繳費則顯示「未付款」
        public string PayTypeDescription
        {
            get
            {
                if (State == 0)  
                    return "-"; 

                return PayType == 0 ? "現金" : (PayType == 1 ? "信用卡" : "未知");
            }
        }

        // 顯示繳費狀態，0為未繳費，1為已繳費
        public string StateDescription
        {
            get
            {
                return State == 1 ? "已繳費" : "未繳費";
            }
        }

        // 顯示付款時間，如果未繳費則顯示「未付款」
        public string PayTimeDescription
        {
            get
            {
                if (State == 0)  
                    return "-";  

                return PayTime?.ToString("yyyy-MM-dd") ?? "未知";  
            }
        }

         
        public int TotalPrice
        {
            get
            {
                return (Price ?? 0) + (MotorPrice ?? 0) + (CarPrice ?? 0);
            }
        }
    }
}
