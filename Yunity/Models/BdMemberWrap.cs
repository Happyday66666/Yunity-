using System.ComponentModel;

namespace Yunity.Models
{
    public class BdMemberWrap
    {
        private BdMember _BdMember;
        private TusersInfo _TusersInfo;
        private BdList _BdList;
        public BdMember BdMember
        {
            get { return _BdMember; }
            set { _BdMember = value; }
        }

        public TusersInfo TusersInfo
        {
            get { return _TusersInfo; }
            set { _TusersInfo = value; }
        }
        public BdList BdList
        {
            get { return _BdList; }
            set { _BdList = value; }
        }
        public BdMemberWrap()
        {
            _BdMember = new BdMember();
            _TusersInfo = new TusersInfo();
            _BdList = new BdList();
        }

        public int? BdId {
            get { return _BdMember.BdId; }
            set { _BdMember.BdId = value; }
        }

        [DisplayName("大樓戶號")]
        public string? DoorNo {
            get { return _BdMember.DoorNo; }
            set { _BdMember.DoorNo = value; }
        }

        public int? UserId {
            get { return _BdMember.UserId; }
            set { _BdMember.UserId = value; }
        }

        [DisplayName("住戶姓名")]
        public string FName
        {
            get { return _TusersInfo.FName; }
            set { _TusersInfo.FName = value; }
        }

        [DisplayName("登入帳號")]
        public string FAccount
        {
            get { return _TusersInfo.FAccount; }
            set { _TusersInfo.FAccount = value; }
        }

        [DisplayName("註冊信箱")]
        public string FEmail
        {
            get { return _TusersInfo.FEmail; }
            set { _TusersInfo.FEmail = value; }
        }

        public int Id {
            get { return _BdList.Id; }
            set { _BdList.Id = value; }
        }

        public string? BdName
        {
            get { return _BdList.BdName; }
            set { _BdList.BdName = value; }
        }

        public string? BdAddr
        {
            get { return _BdList.BdAddr; }
            set { _BdList.BdAddr = value; }
        }
    }
}
