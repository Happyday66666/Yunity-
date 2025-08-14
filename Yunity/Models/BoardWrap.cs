namespace Yunity.Models
{
    public class BoardWrap
    {
        public Board _Board = null;
        public TManagerInfo _TManagerInfo = null;

        public Board Board {
            get { return _Board; }
            set { _Board = value; }
        }
        public TManagerInfo TManagerInfo
        {
            get { return _TManagerInfo; }
            set { _TManagerInfo = value; }
        }

        public BoardWrap ()
        {
            _Board = new Board ();
            _TManagerInfo = new TManagerInfo ();
        }

        public int Id
        {
            get { return _Board.Id; }
            set { _Board.Id = value; }
        }

        public int? BdId
        {
            get { return _Board.BdId; }
            set { _Board.BdId = value; }
        }

        public int? ManagerId
        {
            get { return _Board.ManagerId; }
            set { _Board.ManagerId = value; }
        }

        public int Type
        {
            get { return _Board.Type; }
            set { _Board.Type = value; }
        }

        public string Name
        {
            get { return _Board.Name; }
            set { _Board.Name = value; }
        }

        public string Info
        {
            get { return _Board.Info; }
            set { _Board.Info = value; }
        }

        public string? Photo
        {
            get { return _Board.Photo; }
            set { _Board.Photo = value; }
        }

        public DateTime? OpenTime
        {
            get { return _Board.OpenTime; }
            set { _Board.OpenTime = value; }
        }

        public int? Auth
        {
            get { return _Board.Auth; }
            set { _Board.Auth = value; }
        }
        public int? State
        {
            get { return _Board.State; }
            set { _Board.State = (int)value; }
        }
       
        public string FName
        {
            get { return _TManagerInfo.FName; }
            set { _TManagerInfo.FName = value; }
        }

        public IFormFile? ImageFile { get; set; } 
    }
}
