using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Yunity.Models
{
    public class CCompanyLoginWrap
    {
        public CompanyLoginHistory _companyLG = null;
        public CompanyLoginHistory companyLG 
        {
            get { return _companyLG; }
            set { _companyLG = value; }
        }
        public int Id
        {
            get { return _companyLG.Id; }
            set { _companyLG.Id = value; }
        }

        public int? CompanyId
        {
            get { return _companyLG.CompanyId; }
            set { _companyLG.CompanyId = value; }
        }
        [DisplayName("登入時間")]
        public DateTime? ComLoginTime
        {
            get { return _companyLG.ComLoginTime; }
            set { _companyLG.ComLoginTime = value; }
        }
        [DisplayName("登出時間")]
        public DateTime? ComLogoutTime
        {
            get { return _companyLG.ComLogoutTime; }
            set { _companyLG.ComLogoutTime = value; }
        }
        [DisplayName("IP位址")]
        public string ComIpAddress
        {
            get { return _companyLG.ComIpAddress; }
            set { _companyLG.ComIpAddress = value; }
        }
        [DisplayName("狀態")]
        public string ComLoginStatus
        {
            get { return _companyLG.ComLoginStatus; }
            set { _companyLG.ComLoginStatus = value; }
        }
    }
}
