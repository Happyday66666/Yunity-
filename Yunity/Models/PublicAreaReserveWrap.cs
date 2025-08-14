using System.ComponentModel.DataAnnotations.Schema;

namespace Yunity.Models
{
    public class PublicAreaReserveWrap
    {
        public PublicAreaReserve _PublicAreaReserve;

        public PublicAreaReserve PublicAreaReserve
        {
            get { return _PublicAreaReserve; }
            set { _PublicAreaReserve = value; }
        }

        public PublicAreaReserveWrap()
        {
            _PublicAreaReserve = new PublicAreaReserve();
        }
        public int Id
        {
            get { return _PublicAreaReserve.Id; }
            set { _PublicAreaReserve.Id = value; }
        }

        public int? BdId
        {
            get { return _PublicAreaReserve.BdId; }
            set { _PublicAreaReserve.BdId = value; }
        }

        public string Name
        {
            get { return _PublicAreaReserve.Name; }
            set { _PublicAreaReserve.Name = value; }
        }

        public TimeOnly? OpenTime
        {
            get { return _PublicAreaReserve.OpenTime; }
            set { _PublicAreaReserve.OpenTime = value; }
        }

        public TimeOnly? CloseTime
        {
            get { return _PublicAreaReserve.CloseTime; }
            set { _PublicAreaReserve.CloseTime = value; }
        }

        public int? DeductUnit
        {
            get { return _PublicAreaReserve.DeductUnit; }
            set { _PublicAreaReserve.DeductUnit = value; }
        }

        public string UseTime
        {
            get { return _PublicAreaReserve.UseTime; }
            set { _PublicAreaReserve.UseTime = value; }
        }

        public string AreaInfo
        {
            get { return _PublicAreaReserve.AreaInfo; }
            set { _PublicAreaReserve.AreaInfo = value; }
        }

        public int? Amount
        {
            get { return _PublicAreaReserve.Amount; }
            set { _PublicAreaReserve.Amount = value; }
        }

        public string Photo
        {
            get { return _PublicAreaReserve.Photo; }
            set { _PublicAreaReserve.Photo = value; }
        }

        public int? Icon
        {
            get { return _PublicAreaReserve.Icon; }
            set { _PublicAreaReserve.Icon = value; }
        }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
