namespace Yunity.Models
{
    public class SendPacksWrap
    {
         
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Type { get; set; }  // 修改為 string 類型
            public string GetUser { get; set; }
            public string GetTel { get; set; }
            public string SendAddr { get; set; }
            public string Logistic { get; set; }
            public string PickUser { get; set; }
            public DateTime? PickTime { get; set; }
            public string PackPhoto { get; set; }
            public DateTime? PickLogisticTime { get; set; }
            public string State { get; set; }  // 修改為 string 類型
         

    }
}
