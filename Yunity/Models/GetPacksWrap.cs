namespace Yunity.Models
{
    public class GetPacksWrap
    {
        public int Id { get; set; }

        public string? PackNo { get; set; }

        public string? Type { get; set; }

        public int Bdid { get; set; }
        public int? DoorNoId { get; set; }

        public int? ManagerId { get; set; }

        public string? GetUser { get; set; }

        public string? Logistic { get; set; }

        public DateTime? LogTime { get; set; }

        public string? PackPhoto { get; set; }

        public string? PickUser { get; set; }

        public DateTime? PickTime { get; set; }

        public string? State { get; set; }

        public string? UserName { get; set; } 
    }
}
