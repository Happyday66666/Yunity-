using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class GetPackWrap
    {
        public GetPack _GetPack = null;
        public DoorNo _DoorNo = null;
        public GetPack GetPack
        {
            get { return _GetPack; }
            set { _GetPack = value; }
        }
        public DoorNo doorNo
        {
            get { return _DoorNo; }
            set { _DoorNo = value; }
        }

        public GetPackWrap()
        {
            _GetPack = new GetPack();
            _DoorNo = new DoorNo();
        }
        public int Id
        {
            get { return _GetPack.Id; }
            set { _GetPack.Id = value; }
        }

        [Required(ErrorMessage = "包裹號為必填欄位")]
        public string? PackNo
        {
            get { return _GetPack.PackNo; }
            set { _GetPack.PackNo = value; }
        }

        public int? BdId
        {
            get { return _GetPack.BdId; }
            set { _GetPack.BdId = value; }
        }
        [Required(ErrorMessage = "包裹類型為必填欄位")]
        public int Type
        {
            get { return _GetPack.Type; }
            set { _GetPack.Type = value; }
        }

        public int? DoorNoId
        {
            get { return _GetPack.DoorNoId; }
            set { _GetPack.DoorNoId = value; }
        }

        public int? ManagerId
        {
            get { return _GetPack.ManagerId; }
            set { _GetPack.ManagerId = value; }
        }
        [Required(ErrorMessage = "收件人為必填欄位")]
        public string? GetUser
        {
            get { return _GetPack.GetUser; }
            set { _GetPack.GetUser = value; }
        }

        [Required(ErrorMessage = "物流公司為必填欄位")]
        public string? Logistic
        {
            get { return _GetPack.Logistic; }
            set { _GetPack.Logistic = value; }
        }

        public DateTime? LogTime
        {
            get { return _GetPack.LogTime; }
            set { _GetPack.LogTime = value; }
        }

        public string? PackPhoto
        {
            get { return _GetPack.PackPhoto; }
            set { _GetPack.PackPhoto = value; }
        }

        public string? PickUser
        {
            get { return _GetPack.PickUser; }
            set { _GetPack.PickUser = value; }
        }

        public DateTime? PickTime
        {
            get { return _GetPack.PickTime; }
            set { _GetPack.PickTime = value; }
        }

        public int? State
        {
            get { return _GetPack.State; }
            set { _GetPack.State = value; }
        }

        public string? DoorNo1
        {
            get { return _DoorNo.DoorNo1; }
            set { _DoorNo.DoorNo1 = value; }
        }

        public IFormFile? ImageFile { get; set; }
        [Required(ErrorMessage = "門戶號碼為必填欄位")]
        public string? DoorNo { get; set; }
        public string? ImageUrl { get; set; }
        public string? Poster { get; set; }


    }
}
