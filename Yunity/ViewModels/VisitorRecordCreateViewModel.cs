using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class VisitorRecordCreateViewModel
    {
        [Required(ErrorMessage = "請輸入訪客姓名")]
        public string VisitorName { get; set; }

        [Required(ErrorMessage = "請輸入拜訪時間")]
        public DateTime? VisitTime { get; set; }

        [Required(ErrorMessage = "請輸入拜訪原因")]
        public string VisitReason { get; set; }
    }
}
 
