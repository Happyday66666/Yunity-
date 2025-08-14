using System.ComponentModel.DataAnnotations.Schema;

namespace Yunity.Models
{
    public class SendPackWrap
    {
        public SendPack _SendPack;

        public SendPack SendPack
        {
            get { return _SendPack; }
            set { _SendPack = value; }
        }

        public SendPackWrap()
        {
            _SendPack = new SendPack();
        }
        public int Id
        {
            get { return _SendPack.Id; }
            set { _SendPack.Id = value; }
        }

        public int? UserId
        {
            get { return _SendPack.UserId; }
            set { _SendPack.UserId = value; }
        }

        public int? ManagerId
        {
            get { return _SendPack.ManagerId; }
            set { _SendPack.ManagerId = value; }
        }

        public int? Type
        {
            get { return _SendPack.Type; }
            set { _SendPack.Type = value; }
        }

        public string? GetUser
        {
            get { return _SendPack.GetUser; }
            set { _SendPack.GetUser = value; }
        }

        public string? GetTel
        {
            get { return _SendPack.GetTel; }
            set { _SendPack.GetTel = value; }
        }

        public string? SendAddr
        {
            get { return _SendPack.SendAddr; }
            set { _SendPack.SendAddr = value; }
        }

        public string? Logistic
        {
            get { return _SendPack.Logistic; }
            set { _SendPack.Logistic = value; }
        }

        public string? PickUser
        {
            get { return _SendPack.PickUser; }
            set { _SendPack.PickUser = value; }
        }

        public DateTime? PickTime
        {
            get { return _SendPack.PickTime; }
            set { _SendPack.PickTime = value; }
        }

        public string? PackPhoto
        {
            get { return _SendPack.PackPhoto; }
            set { _SendPack.PackPhoto = value; }
        }

        public DateTime? PickLogisticTime
        {
            get { return _SendPack.PickLogisticTime; }
            set { _SendPack.PickLogisticTime = value; }
        }
        public int? State
        {
            get { return _SendPack.State; }
            set { _SendPack.State = value; }
        }

        public string? FAspUserId { get; set; }


        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public string? QRCord { get; set; }
    }
}
