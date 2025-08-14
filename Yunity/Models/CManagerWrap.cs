using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using Yunity.ViewModels;

namespace Yunity.Models
{
    public class CManagerWrap
    {
        private TManagerInfo _manager;
        private BdList _bdlist;
        public TManagerInfo manager
        {
            get { return _manager; }
            set { _manager = value; }
        }
        public CManagerWrap()
        {
            _manager = new TManagerInfo();
            _bdlist = new BdList();
        }

        public BdList bdlist
        {
            get { return _bdlist; }
            set { _bdlist = value; } 
        }
        public int FId {
            get { return _manager.FId; }
            set {_manager.FId = value; } 
        }
        [DisplayName("姓名")]
        public string FName {
            get { return _manager.FName; }
            set {_manager.FName =value; } 
        }

        [DisplayName("帳號")]
        public string FAccount { 
            get {return _manager.FAccount; } 
            set {_manager.FAccount = value; }
        }

        public string FPassword { 
            get {return _manager.FPassword; }
            set {_manager.FPassword= value; } 
        }
        [DisplayName("大樓ID")]
        public int FBuildingId { 
            get {return _manager.FBuildingId; } 
            set {_manager.FBuildingId = value; } 
        }
        [DisplayName("大樓名稱")]
        public string? BuildingName { 
            get {return _bdlist.BdName; } 
            set {_bdlist.BdName = value; } 
        }

        [DisplayName("電話")]
        public string FPhone { 
            get {return _manager.FPhone; } 
            set {_manager.FPhone = value; } 
        }
        [DisplayName("電子信箱")]
        public string FEmail { 
            get {return _manager.FEmail; } set {_manager.FEmail = value; } 
        }  
         public List<CKeywordViewModel> cKeywords { get; set; }
        
         public  string IsApproved { get; set; }
        public string FAspUserId { get; set; }

        // 下拉選單列表
        public IEnumerable<SelectListItem> BuildingsList { get; set; }
    }
}
