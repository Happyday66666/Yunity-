using System.ComponentModel;
using Yunity.ViewModels;

namespace Yunity.Models
{

    public class CSListWrap
    {
        private CsProduct _CsProduct;
        private CsAppointmentRecord _CsAppointmentRecord;
        
        private CsOrderPhoto _CsOrderPhoto;
        private CsProductPhoto _CsProductPhoto;

        private CompanyAccount _CompanyAccount;
        private CompanyProfile _CompanyProfile;

        public CsProduct CsProduct
        {
            get { return _CsProduct; }
            set { _CsProduct = value; }
        }

        public CsAppointmentRecord CsAppointmentRecord
        {
            get { return _CsAppointmentRecord; }
            set { _CsAppointmentRecord = value; }
        }

        public CompanyAccount CompanyAccount
        {
            get { return _CompanyAccount; }
            set { _CompanyAccount = value; }
        }

        private CompanyProfile CompanyProfile
        {
            get { return _CompanyProfile; }
            set { _CompanyProfile = value; }
        }

        private CsOrderPhoto CsOrderPhoto
        {
            get { return CsOrderPhoto; }
            set { _CsOrderPhoto = value; }
        }

        private CsProductPhoto CsProductPhoto
        {
            get { return _CsProductPhoto; }
            set { _CsProductPhoto = value; }
        }


        public CSListWrap()
        {
            _CsProduct = new CsProduct();
            _CsAppointmentRecord = new CsAppointmentRecord();
            _CompanyAccount = new CompanyAccount();
            _CompanyProfile = new CompanyProfile();
            _CsOrderPhoto = new CsOrderPhoto();
            _CsProductPhoto = new CsProductPhoto();            
        }

        // LIST顯示的畫面

        public int Id
        {
            get { return _CsAppointmentRecord.Id; }
            set { _CsAppointmentRecord.Id = value; }
        }

        [DisplayName("廠商名稱")]
        public string? ComName
        {
            get { return _CompanyAccount.ComName; }
            set { _CompanyAccount.ComName = value; }
        }

        [DisplayName("服務項目")]
        public string? ProductName
        {
            get { return _CsProduct.PName; }
            set { _CsProduct.PName = value; }
        }

        [DisplayName("建立日期")]
        public DateTime? CreatedDate
        {
            get { return _CsAppointmentRecord.CreatedDate; }
            set { _CsAppointmentRecord.CreatedDate = value; }
        }

        [DisplayName("狀態")]
        public string? Status
        {
            get { return _CsAppointmentRecord.Status; }
            set { _CsAppointmentRecord.Status = value; }
        }

        // Detail顯示的畫面
        // 廠商連絡電話、客戶姓名、電話、服務地址、mail、給廠商的話(備註)、提供給照片給廠商、廠商備註

        [DisplayName("廠商連絡電話")]
        public string? ComPhone
        {
            get { return _CompanyProfile.ComPhone; }
            set { _CompanyProfile.ComPhone = value; }
        }

        [DisplayName("姓名")]
        public string? CustomerName
        {
            get { return _CsAppointmentRecord.CustomerName; }
            set { _CsAppointmentRecord.CustomerName = value; }
        }

        [DisplayName("電話")]
        public string? Phone
        {
            get { return _CsAppointmentRecord.Phone; }
            set { _CsAppointmentRecord.Phone = value; }
        }

        [DisplayName("服務地址")]
        public string? ServiceLocation
        {
            get { return _CsAppointmentRecord.ServiceLocation; }
            set { _CsAppointmentRecord.ServiceLocation = value; }
        }

        [DisplayName("Email")]
        public string? Email
        {
            get { return _CsAppointmentRecord.Email; }
            set { _CsAppointmentRecord.Email = value; }
        }

        [DisplayName("給廠商的話")]
        public string? ArNotes
        {
            get { return _CsAppointmentRecord.ArNotes; }
            set { _CsAppointmentRecord.ArNotes = value; }
        }
                
        public string? PhotoId
        {
            get { return _CsOrderPhoto.PhotoId; }
            set { _CsOrderPhoto.PhotoId = value; }
        }


        // 新增的照片屬性
        [DisplayName("提供的照片")]
        public string? Photo1 
        {

            get { return _CsOrderPhoto.PhotoId; }
            set { _CsOrderPhoto.PhotoId = value; }

        }
               
        public string? Photo2 { get; set; }
               
        public string? Photo3 { get; set; }

        // Detail的回饋
        [DisplayName("給廠商的回饋")]
        public string? Feedback {
            get { return _CsAppointmentRecord.Feedback; }
            set { _CsAppointmentRecord.Feedback = value; }
        }

        [DisplayName("完工日期")]
        public DateTime? FinishDate
        {
            get { return _CsAppointmentRecord.FinishDate; }
            set { _CsAppointmentRecord.FinishDate = value; }
        }

    }
}
