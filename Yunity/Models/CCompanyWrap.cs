using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class CCompanyWrap
    {
        public CompanyServiceArea _comAR = null;
        public CompanyProfile _companyPF = null;
        public CompanyAccount _companyAC = null;

        public CompanyServiceArea ComAR { 
            get { return _comAR; }
            set { _comAR = value; }
        }
        public CCompanyWrap()
        {
            _comAR = new CompanyServiceArea();
            _companyPF = new CompanyProfile();
            _companyAC = new CompanyAccount();

        }

        public int ARId
        {
            get { return _comAR.Id; }
            set { _comAR.Id = value; }
        }

        public int? ComId
        {
            get { return _comAR.ComId; }
            set { _comAR.ComId = value; }
        }

        [DisplayName("宜蘭縣")]
        public int? YilanC
        {
            get { return _comAR.YilanC; }
            set { _comAR.YilanC = value; }
        }
        [DisplayName("基隆市")]
        public int? KeelungCity
        {
            get { return _comAR.KeelungCity; }
            set { _comAR.KeelungCity = value; }
        }
        [DisplayName("台北市")]
        public int? TaipeiCity
        {
            get { return _comAR.TaipeiCity; }
            set { _comAR.TaipeiCity = value; }
        }
        [DisplayName("新北市")]
        public int? NewTaipeiCity
        {
            get { return _comAR.NewTaipeiCity; }
            set { _comAR.NewTaipeiCity = value; }
        }
        [DisplayName("桃園市")]
        public int? TaoyuanCity
        {
            get { return _comAR.TaoyuanCity; }
            set { _comAR.TaoyuanCity = value; }
        }
        [DisplayName("新竹市")]
        public int? HsinchuCity
        {
            get { return _comAR.HsinchuCity; }
            set { _comAR.HsinchuCity = value; }
        }
        [DisplayName("新竹縣")]
        public int? HsinchuC
        {
            get { return _comAR.HsinchuC; }
            set { _comAR.HsinchuC = value; }
        }
        [DisplayName("苗栗縣")]
        public int? MiaoliC
        {
            get { return _comAR.MiaoliC; }
            set { _comAR.MiaoliC = value; }
        }
        [DisplayName("台中市")]
        public int? TaichungCity
        {
            get { return _comAR.TaichungCity; }
            set { _comAR.TaichungCity = value; }
        }
        [DisplayName("彰化縣")]
        public int? ChanghuaC
        {
            get { return _comAR.ChanghuaC; }
            set { _comAR.ChanghuaC = value; }
        }
        [DisplayName("南投縣")]
        public int? NantouC
        {
            get { return _comAR.NantouC; }
            set { _comAR.NantouC = value; }
        }
        [DisplayName("雲林縣")]
        public int? YunlinC
        {
            get { return _comAR.YunlinC; }
            set { _comAR.YunlinC = value; }
        }
        [DisplayName("嘉義市")]
        public int? ChiayiCity
        {
            get { return _comAR.ChiayiCity; }
            set { _comAR.ChiayiC = value; }
        }
        [DisplayName("嘉義縣")]
        public int? ChiayiC
        {
            get { return _comAR.ChiayiC; }
            set { _comAR.ChiayiC = value; }
        }
        [DisplayName("台南市")]
        public int? TainanCity
        {
            get { return _comAR.TainanCity; }
            set { _comAR.TainanCity = value; }
        }
        [DisplayName("高雄市")]
        public int? KaohsiungCity
        {
            get { return _comAR.KaohsiungCity; }
            set { _comAR.KaohsiungCity = value; }
        }
        [DisplayName("屏東縣")]
        public int? PingtungC
        {
            get { return _comAR.PingtungC; }
            set { _comAR.PingtungC = value; }
        }
        [DisplayName("澎湖縣")]
        public int? PenghuC
        {
            get { return _comAR.PenghuC; }
            set { _comAR.PenghuC = value; }
        }
        [DisplayName("花蓮縣")]
        public int? HualienC
        {
            get { return _comAR.HualienC; }
            set { _comAR.HualienC = value; }
        }
        [DisplayName("台東縣")]
        public int? TaitungC
        {
            get { return _comAR.TaitungC; }
            set { _comAR.TaitungC = value; }
        }
        [DisplayName("金門縣")]
        public int? KinmenC
        {
            get { return _comAR.KinmenC; }
            set { _comAR.KinmenC = value; }
        }
        [DisplayName("連江縣")]
        public int? LienchiangC
        {
            get { return _comAR.LienchiangC; }
            set { _comAR.LienchiangC = value; }
        }

        public CompanyProfile companyIF
        {
            get { return _companyPF; }
            set { _companyPF = value; }
        }
        
        public int IFid
        {
            get { return _companyPF.Id; }
            set { _companyPF.Id = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("廠商地址")]
        public string ComAddress
        {
            get { return _companyPF.ComAddress; }
            set { _companyPF.ComAddress = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("聯絡人")]
        public string ComPerson
        {
            get { return _companyPF.ComPerson; }
            set { _companyPF.ComPerson = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("連絡電話")]
        public string ComPhone
        {
            get { return _companyPF.ComPhone; }
            set { _companyPF.ComPhone = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("Email")]
        public string ComEmail
        {
            get { return _companyPF.ComEmail; }
            set { _companyPF.ComEmail = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("服務項目")]
        public string ComServerItem
        {
            get { return _companyPF.ComServerItem; }
            set { _companyPF.ComServerItem = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("統一編號")]
        public int? ComRegistrationNumber
        {
            get { return _companyPF.ComRegistrationNumber; }
            set { _companyPF.ComRegistrationNumber = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("合約起日")]
        public DateTime? ComContractStartDate
        {
            get { return _companyPF.ComContractStartDate; }
            set { _companyPF.ComContractStartDate = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("合約迄日")]
        public DateTime? ComContractEndDate
        {
            get { return _companyPF.ComContractEndDate; }
            set { _companyPF.ComContractEndDate = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("營業時間")]
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
        [Required(ErrorMessage = "必填")]
        [DisplayName("權限")]
        public int? ComPermissions
        {
            get { return _companyAC.ComPermissions; }
            set { _companyAC.ComPermissions = value; }
        }
        [Required(ErrorMessage = "必填")]
        [DisplayName("狀態")]
        public int? ComStatus
        {
            get { return _companyAC.ComStatus; }
            set { _companyAC.ComStatus = value; }
        }
    }


}
