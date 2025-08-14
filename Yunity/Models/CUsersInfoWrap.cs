using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Security.Policy;

namespace Yunity.Models
{
    public class CUsersInfoWrap
    {
        private TusersInfo _usersInfo = null;
        private BdList _BdList = null;
        private BdMember _BdMember = null;

        public TusersInfo Users
        {
            get { return _usersInfo; }
            set { _usersInfo = value; }
        }
        public BdList Bd
        {
            get { return _BdList; }
            set { _BdList = value; }
        }
        public BdMember bm
        {
            get { return _BdMember; }
            set { _BdMember = value; }
        }
        public CUsersInfoWrap()
        {
            _usersInfo = new TusersInfo();
            _BdList = new BdList();
            _BdMember = new BdMember();
            BuildingNames = new List<string>();
        }

        public int FId
        {
            get { return _usersInfo.FId; }
            set { _usersInfo.FId = value; }
        }

        [DisplayName("姓名")]
        public string? FName
        {
            get { return _usersInfo.FName; }
            set { _usersInfo.FName = value; }
        }
        
        public string? FAccount
        {
            get { return _usersInfo.FAccount; }
            set { _usersInfo.FAccount = value; }
        }
        public string? FPassword
        {
            get { return _usersInfo.FPassword; }
            set { _usersInfo.FPassword = value; }
        }
        [DisplayName("電話")]
        public string? FPhone
        {
            get { return _usersInfo.FPhone; }
            set { _usersInfo.FPhone = value; }
        }

        public int FBuildingId
        {
            get { return _usersInfo.FBuildingId; }
            set { _usersInfo.FBuildingId = value; }
        }
        [DisplayName("聯絡地址")]
        public string? FUserAddress
        {
            get { return _usersInfo.FUserAddress; }
            set { _usersInfo.FUserAddress = value; }

        }
        [DisplayName("電子郵件")]
        public string? FEmail
        {
            get { return _usersInfo.FEmail; }
            set { _usersInfo.FEmail = value; }
        }
        [DisplayName("所屬大樓")]
        public string? BdName
        {
            get { return _BdList.BdName; }
            set { _BdList.BdName = value; }
        }
        public List<string?> BuildingNames { get; set; }
      
    }
}
