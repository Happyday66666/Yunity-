using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class CServHistoryWrap
    {
        public ServerHistory _CompanySH = null;
        public CompanyAccount _CompanyAC = null;
        public CsAppointmentRecord _CSAR = null; //預約紀錄
        public CsProduct _CSP = null; //預約的服務項目

        public ServerHistory CompanySH 
        { 
            get{ return _CompanySH; } 
            set{ _CompanySH = value; }
        }
        public CompanyAccount companyAC
        {
            get { return _CompanyAC; }
            set { _CompanyAC = value; }
        }

        public CsAppointmentRecord CSAR //預約紀錄
        {
            get { return _CSAR; }
            set { _CSAR = value;  }
        }

        public CsProduct CSP //預約的服務項目
        {
            get { return _CSP; }
            set { _CSP = value; }
        }

        public CServHistoryWrap()
        {
            _CompanySH = new ServerHistory();
            _CompanyAC = new CompanyAccount();
            _CSAR = new CsAppointmentRecord();
            _CSP = new CsProduct();
        }
        public int Id
        {
            get { return _CSAR.Id; }
            set { _CSAR.Id = value; }
        }

        public int? Comid
        {
            get { return _CSP.ComId; }
            set { _CSP.ComId = value; }
        }
        [DisplayName("訂單日期")] //預約紀錄的建立日期
        public DateTime? ServiceDate
        {
            get { return _CSAR.CreatedDate; }
            set { _CSAR.CreatedDate = value; }
        }
        [DisplayName("服務項目")] //預約紀錄的服務項目(產品名稱)
        public string ServiceType
        {
            get { return _CSP.PCategory; }
            set { _CSP.PCategory = value; }
        }
        [DisplayName("服務對象")] //預約紀錄的姓名
        public string ServiceWho
        {
            get { return _CSAR.CustomerName; }
            set { _CSAR.CustomerName = value; }
        }
        [DisplayName("狀態")] //預約紀錄的狀態
        public string ServiceStatus
        {
            get { return _CSAR.Status; }
            set { _CSAR.Status = value; }
        }
        [DisplayName("評價")] //預約紀錄的回饋
        public string ServiceFeedback
        {
            get { return _CSAR.Feedback; }
            set { _CSAR.Feedback = value; }
        }
        public int CAid
        {
            get { return _CompanyAC.Id; }
            set { _CompanyAC.Id = value; }
        }
        [DisplayName("公司名稱")]
        public string ComName
        {
            get { return _CompanyAC.ComName; }
            set { _CompanyAC.ComName = value; }
        }


    }
}
