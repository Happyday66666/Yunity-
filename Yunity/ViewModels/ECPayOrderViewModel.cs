namespace Yunity.ViewModels
{
    public class ECPayOrderViewModel
    {
        public int Door_No_Id { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string PaymentType { get; set; }
        public decimal TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string CheckMacValue { get; set; }
        public string ReturnURL { get; set; }
        public string OrderResultURL { get; set; }
        public string PaymentInfoURL { get; set; }
        public string ClientRedirectURL { get; set; }
    }
}
