namespace Yunity.Models
{
    public class CalendarWrap
    {
        public Calendar _Calendar = null;
        public UserAreaReserve _UserAreaReserve = null;

        public Calendar Calendar
        {
            get { return _Calendar; }
            set { _Calendar = value; }
        }
        public UserAreaReserve UserAreaReserve
        {
            get { return _UserAreaReserve; }
            set { _UserAreaReserve = value; }
        }
        public CalendarWrap() 
        {
            _Calendar = new Calendar();
            _UserAreaReserve = new UserAreaReserve();
        }
        public int Id
        {
            get { return _Calendar.Id; }
            set { _Calendar.Id = value; }
        }

        public int? BdId
        {
            get { return _Calendar.BdId; }
            set { _Calendar.BdId = value; }
        }

        public int? ManagerId
        {
            get { return _Calendar.ManagerId; }
            set { _Calendar.ManagerId = value; }
        }

        public DateTime? EventStart
        {
            get { return _Calendar.EventStart; }
            set { _Calendar.EventStart = value; }
        }

        public string EventName
        {
            get { return _Calendar.EventName; }
            set { _Calendar.EventName = value; }
        }

        public DateTime? EventEnd
        {
            get { return _Calendar.EventEnd; }
            set { _Calendar.EventEnd = value; }
        }

        public string Info
        {
            get { return _Calendar.Info; }
            set { _Calendar.Info = value; }
        }

        public DateTime? LogTime
        {
            get { return _Calendar.LogTime; }
            set { _Calendar.LogTime = value; }
        }

        public int? AreaId
        {
            get { return _UserAreaReserve.AreaId; }
            set { _UserAreaReserve.AreaId = value; }
        }

        public int? DoorNoId
        {
            get { return _UserAreaReserve.DoorNoId; }
            set { _UserAreaReserve.DoorNoId = value; }
        }

        public int UserId
        {
            get { return (int)_UserAreaReserve.UserId; }
            set { _UserAreaReserve.UserId = value; }
        }

        public DateTime? ReserveTime
        {
            get { return _UserAreaReserve.ReserveTime; }
            set { _UserAreaReserve.ReserveTime = value; }
        }

        public DateTime? StartTime
        {
            get { return _UserAreaReserve.StartTime; }
            set { _UserAreaReserve.StartTime= value; }
        }

        public DateTime? EndTime
        {
            get { return _UserAreaReserve.EndTime; }
            set { _UserAreaReserve.EndTime = value; }
        }

        public int? State
        {
            get { return _UserAreaReserve.State; }
            set { _UserAreaReserve.State = value; }
        }

    }
}
