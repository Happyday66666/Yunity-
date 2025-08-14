using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class CComProfileWrap
    {
        // 宣告私有欄位,儲存實例
        public CompanyAccount _CompanyAC = null;
        public CompanyProfile _CompanyPF = null;
        public CompanyServiceArea _ComAR = null;

        // 建構子 (Constructor):初始化實例
        public CComProfileWrap()
        {
            _CompanyAC = new CompanyAccount();
            _CompanyPF = new CompanyProfile();
            _ComAR = new CompanyServiceArea();
        }

        // 屬性 (Properties):欄位的控制與管理
        // CompanyAccount
        public CompanyAccount AcountIF
        {
            get { return _CompanyAC; }
            set { _CompanyAC = value; }
        }
        public int CompanyACId
        {
            get { return _CompanyAC.Id; }
            set { _CompanyAC.Id = value; }
        }
        // 資料屬性 (Data Properties): 對應具體實例欄位
        [DisplayName("公司名稱")]
        public string ComName
        {
            get { return _CompanyAC.ComName; }
            set { _CompanyAC.ComName = value; }
        }
        [DisplayName("狀態")]
        public int? ComStatus
        {
            get { return _CompanyAC.ComStatus; }
            set { _CompanyAC.ComStatus = value; }
        }

        // CompanyProfile
        public CompanyProfile ProfileIF
        {
            get { return _CompanyPF; }
            set { _CompanyPF = value; }
        }
        public int CompanyPFId
        {
            get { return _CompanyPF.Id; }
            set { _CompanyPF.Id = value; }
        }
        public int? CompanyPFComId
        {
            get { return _CompanyPF.ComId; }
            set { _CompanyPF.ComId = value; }
        }

        [DisplayName("地址")]
        public string ComAddress
        {
            get { return _CompanyPF.ComAddress; }
            set { _CompanyPF.ComAddress = value; }
        }
        [DisplayName("聯絡人")]
        public string ComPerson
        {
            get { return _CompanyPF.ComPhone; }
            set { _CompanyPF.ComPerson = value; }
        }
        [DisplayName("連絡電話")]
        public string ComPhone
        {
            get { return _CompanyPF.ComPhone; }
            set { _CompanyPF.ComPhone = value; }
        }
        [DisplayName("E-mail")]
        public string ComEmail
        {
            get { return _CompanyPF.ComEmail; }
            set { _CompanyPF.ComEmail = value; }
        }
        [DisplayName("服務項目")]
        public string ComServerItem
        {
            get { return _CompanyPF.ComServerItem; }
            set { _CompanyPF.ComServerItem = value; }
        }
        [DisplayName("統一編號")]
        public int? ComRegistrationNumber
        {
            get { return _CompanyPF.ComRegistrationNumber; }
            set { _CompanyPF.ComRegistrationNumber = value; }
        }
        [DisplayName("合約起日")]
        public DateTime? ComContractStartDate
        {
            get { return _CompanyPF.ComContractStartDate; }
            set { _CompanyPF.ComContractStartDate = value; }
        }
        [DisplayName("合約迄日")]
        public DateTime? ComContractEndDate
        {
            get { return _CompanyPF.ComContractEndDate; }
            set { _CompanyPF.ComContractEndDate = value; }
        }
        
        [DisplayName("營業時間")]
        public string ComBusinessHours
        {
            get { return _CompanyPF.ComBusinessHours; }
            set { _CompanyPF.ComBusinessHours = value; }
        }
        [DisplayName("帳號")]
        public string ComAccount
        {
            get { return _CompanyAC.ComAccount; }
            set { _CompanyAC.ComAccount = value; }
        }

        // CompanyServiceArea
        public CompanyServiceArea ServiceAreaIF
        {
            get { return _ComAR; }
            set { _ComAR = value; }
        }
    }
}
