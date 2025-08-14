using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class VisitorRecordWrap
    {
        public VisitorRecord _VisitorRecord = null;
        public DoorNo _DoorNo = null;

        public VisitorRecord VisitorRecord
        {
            get { return _VisitorRecord; }
            set { _VisitorRecord = value; }
        }
        public DoorNo doorNo
        {
            get { return _DoorNo; }
            set { _DoorNo = value; }
        }
        public VisitorRecordWrap()
        {
            _VisitorRecord = new VisitorRecord();
            _DoorNo = new DoorNo();

        }

        public int Id
        {
            get { return _VisitorRecord.Id; }
            set { _VisitorRecord.Id = value; }
        }

        public int? DoorNoId
        {
            get { return _VisitorRecord.DoorNoId; }
            set { _VisitorRecord.DoorNoId = value; }
        }

        public int? ManagerId
        {
            get { return _VisitorRecord.ManagerId; }
            set { _VisitorRecord.ManagerId = value; }
        }
        [Required(ErrorMessage = "請輸入訪客姓名")]
        public string VisitorName
        {
            get { return _VisitorRecord.VisitorName; }
            set { _VisitorRecord.VisitorName = value; }
        }

        public DateTime? VisitTime
        {
            get { return _VisitorRecord.VisitTime; }
            set { _VisitorRecord.VisitTime = value; }
        }
        [Required(ErrorMessage = "請輸入原因")]
        public string VisitReason
        {
            get { return _VisitorRecord.VisitReason; }
            set { _VisitorRecord.VisitReason = value; }
        }
        public int? BdId
        {
            get { return _VisitorRecord.BdId; }
            set { _VisitorRecord.BdId = value; }
        }

        [Required(ErrorMessage = "請輸入拜訪門戶號")]
        public string DoorNum { get; set; }

    }
}
