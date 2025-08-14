using System.ComponentModel;

namespace Yunity.Models
{
    public class BdManagerWrap
    {
        private BdManager _BdManager;
        private TManagerInfo _TManagerInfo;
        private BdList _BdList;
        public BdManager BdManager
        {
            get { return _BdManager; }
            set { _BdManager = value; }
        }

        public TManagerInfo TManagerInfo
        {
            get { return _TManagerInfo; }
            set { _TManagerInfo = value; }
        }
        public BdList BdList
        {
            get { return _BdList; }
            set { _BdList = value; }
        }
        public BdManagerWrap()
        {
            _BdManager = new BdManager();
            _TManagerInfo = new TManagerInfo();
            _BdList = new BdList();
        }
        public int Id
        {
            get { return _BdManager.Id; }
            set { _BdManager.Id = value; }
        }

        public int? BdId {
            get { return _BdManager.BdId; }
            set { _BdManager.BdId = value; }
        }

        public int? ManagerId {
            get { return _BdManager.ManagerId; }
            set { _BdManager.ManagerId = value; }
        }

        [DisplayName("管理者權限")]
        public int? Permissions {
            get { return _BdManager.Permissions; }
            set { _BdManager.Permissions = value; }
        }

        [DisplayName("管理者姓名")]
        public string FName
        {
            get { return _TManagerInfo.FName; }
            set { _TManagerInfo.FName = value; }
        }

        [DisplayName("管理者帳號")]
        public string FAccount
        {
            get { return _TManagerInfo.FAccount; }
            set { _TManagerInfo.FAccount = value; }
        }

        [DisplayName("管理者信箱")]
        public string FEmail
        {
            get { return _TManagerInfo.FEmail; }
            set { _TManagerInfo.FEmail = value; }
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
