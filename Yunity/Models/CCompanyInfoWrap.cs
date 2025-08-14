using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class CCompanyInfoWrap
    {
        public CompanyProfile _companyPF = null;
        public CompanyAccount _companyAC = null;

        public CompanyProfile companyIF
        {
            get { return _companyPF; }
            set { _companyPF = value; }
        }
        public CCompanyInfoWrap()
        {
            _companyPF = new CompanyProfile();
            _companyAC = new CompanyAccount();
        }
        public int IFid
        {
            get { return _companyPF.Id; }
            set { _companyPF.Id = value; }
        }

        public int? ComId
        {
            get { return _companyPF.ComId; }
            set { _companyPF.ComId = value; }
        }
        [DisplayName("廠商地址")]
        public string ComAddress
        {
            get { return _companyPF.ComAddress; }
            set { _companyPF.ComAddress = value; }
        }
        [DisplayName("聯絡人")]
        public string ComPerson
        {
            get { return _companyPF.ComPerson; }
            set { _companyPF.ComPerson = value; }
        }
        [DisplayName("連絡電話")]
        public string ComPhone
        {
            get { return _companyPF.ComPhone; }
            set { _companyPF.ComPhone = value; }
        }
        [DisplayName("Email")]
        public string ComEmail
        {
            get { return _companyPF.ComEmail; }
            set { _companyPF.ComEmail = value; }
        }
        [DisplayName("服務項目")]
        public string ComServerItem
        {
            get { return _companyPF.ComServerItem; }
            set { _companyPF.ComServerItem = value; }
        }

        public int? ComRegistrationNumber
        {
            get { return _companyPF.ComRegistrationNumber; }
            set { _companyPF.ComRegistrationNumber = value; }
        }
        [DisplayName("合約起日")]
        public DateTime? ComContractStartDate
        {
            get { return _companyPF.ComContractStartDate; }
            set { _companyPF.ComContractStartDate = value; }
        }
        [DisplayName("合約迄日")]
        public DateTime? ComContractEndDate
        {
            get { return _companyPF.ComContractEndDate; }
            set { _companyPF.ComContractEndDate = value; }
        }

        public string ComBusinessHours
        {
            get { return _companyPF.ComBusinessHours; }
            set { _companyPF.ComBusinessHours = value; }
        }

        public CompanyAccount companyAC
        {
            get { return _companyAC; }
            set { _companyAC = value; }
        }

        public int CAid
        {
            get { return _companyAC.Id; }
            set { _companyAC.Id = value; }
        }

        [Required(ErrorMessage = "必填")]
        [DisplayName("公司名稱")]
        public string ComName
        {
            get { return _companyAC.ComName; }
            set { _companyAC.ComName = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("帳號")]
        public string ComAccount
        {
            get { return _companyAC.ComAccount; }
            set { _companyAC.ComAccount = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("密碼")]
        public string ComPassword
        {
            get { return _companyAC.ComPassword; }
            set { _companyAC.ComPassword = value; }
        }
        [DisplayName("權限")]
        public int? ComPermissions
        {
            get { return _companyAC.ComPermissions; }
            set { _companyAC.ComPermissions = value; }
        }
        [DisplayName("狀態")]
        public int? ComStatus
        {
            get { return _companyAC.ComStatus; }
            set { _companyAC.ComStatus = value; }
        }
    }
}
