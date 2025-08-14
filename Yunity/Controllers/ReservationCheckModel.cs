namespace Yunity.Controllers
{
    public class ReservationCheckModel
    {
        public int AreaId { get; set; }
        public string StartTime { get; set; }  // 作为字符串传递，后端转换为 DateTime
        public string EndTime { get; set; }    // 作为字符串传递，后端转换为 DateTime
       
    }
}