namespace Yunity.Models
{
    public class UserAreaReserveWithAreaInfo
    {
        public int Id { get; set; }
        public int? AreaId { get; set; }
        public int? DoorNoId { get; set; }
        public int? UserId { get; set; }
        public DateTime? ReserveTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? State { get; set; }
        public PublicAreaReserve PublicArea { get; set; }
        public int? ReservedPeople { get; internal set; }
    }
}