using System.ComponentModel;

namespace Yunity.Models
{
    public class BdListWrap
    {
        private BdList _BdList;
        private BdPermission _BdPermission;

        public BdList BdList
        {
            get { return _BdList; }
            set { _BdList = value; }
        }
        public BdPermission BdPermission
        {
            get { return _BdPermission; }
            set { _BdPermission = value; }
        }

        public BdListWrap()
        {
            _BdList = new BdList();
            _BdPermission = new BdPermission();
        }

        public int Id {
            get { return _BdList.Id; }
            set { _BdList.Id = value; }
        }

        [DisplayName("大樓名稱")]
        public string? BdName {
            get { return _BdList.BdName; }
            set { _BdList.BdName = value; }
        }

        [DisplayName("大樓地址")]
        public string? BdAddr {
            get { return _BdList.BdAddr; }
            set { _BdList.BdAddr = value; }
        }

        [DisplayName("合約起日")]
        public DateTime? ContractStart {
            get { return _BdList.ContractStart; }
            set { _BdList.ContractStart = value; }
        }

        [DisplayName("合約迄日")]
        public DateTime? ContractEnd
        {
            get { return _BdList.ContractEnd; }
            set { _BdList.ContractEnd = value; }
        }

        [DisplayName("主要聯絡人")]
        public string? FirstContact {
            get { return _BdList.FirstContact; }
            set { _BdList.FirstContact = value; }
        }

        [DisplayName("連絡電話")]
        public string? FstContactTel {
            get { return _BdList.FstContactTel; }
            set { _BdList.FstContactTel = value; }
        }

        [DisplayName("次要聯絡人")]
        public string? SecondContact {
            get { return _BdList.SecondContact; }
            set { _BdList.SecondContact = value; }
        }

        [DisplayName("連絡電話")]
        public string? SecContactTel {
            get { return _BdList.SecContactTel; }
            set { _BdList.SecContactTel = value; }
        }

        [DisplayName("戶數")]
        public string? HouseCount{
            get { return _BdList.HouseCount; }
            set { _BdList.HouseCount = value; }
        }

        [DisplayName("狀態")]
        public int? BdState {
            get { return _BdList.BdState; }
            set { _BdList.BdState = value; }
        }

        public int? BdId {
            get { return _BdPermission.BdId; }
            set { _BdPermission.BdId = value; }
        }

        [DisplayName("包裹管理")]
        public int? ReceivePackage {
            get { return _BdPermission.ReceivePackage; }
            set { _BdPermission.ReceivePackage = value; }
        }

        [DisplayName("包裹寄送")]
        public int? SentPackage {
            get { return _BdPermission.SentPackage; }
            set { _BdPermission.SentPackage = value; }
        }

        [DisplayName("管理費管理")]
        public int? ManageFee {
            get { return _BdPermission.ManageFee; }
            set { _BdPermission.ManageFee = value; }
        }

        [DisplayName("社區布告欄")]
        public int? Bulletin {
            get { return _BdPermission.Bulletin; }
            set { _BdPermission.Bulletin = value; }
        }

        [DisplayName("公設預約")]
        public int? PublicAreaReserve {
            get { return _BdPermission.PublicAreaReserve; }
            set { _BdPermission.PublicAreaReserve = value; }
        }

        [DisplayName("附近店家")]
        public int? NearbyStore {
            get { return _BdPermission.NearbyStore; }
            set { _BdPermission.NearbyStore = value; }
        }

        [DisplayName("社區服務")]
        public int? CommunityService {
            get { return _BdPermission.CommunityService; }
            set { _BdPermission.CommunityService = value; }
        }

        [DisplayName("意見回饋")]
        public int? Feedback {
            get { return _BdPermission.Feedback; }
            set { _BdPermission.Feedback = value; }
        }

        [DisplayName("訪客紀錄")]
        public int? VisitorRecord {
            get { return _BdPermission.VisitorRecord; }
            set { _BdPermission.VisitorRecord = value; }
        }

        [DisplayName("社區行事曆")]
        public int? Calendar {
            get { return _BdPermission.Calendar; }
            set { _BdPermission.Calendar = value; }
        }

        [DisplayName("住戶數")]
        public string? Usercount {get ; set;}

        [DisplayName("管理者")]
        public int? Managercount { get; set; }
    }
}
