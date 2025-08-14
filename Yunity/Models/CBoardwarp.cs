namespace Yunity.Models
{
    public class CBoardwarp
    {
        public string BdName { get; set; }  //大樓名稱

        public string FName { get; set; }   //管理員名稱
        public int Id { get; set; }

        public int? BdId { get; set; }

        public int? ManagerId { get; set; }

        public int? Type { get; set; }

        public string? Name { get; set; }

        public string? Info { get; set; }

        public string? Photo { get; set; }

        public DateTime? OpenTime { get; set; }

        public int? Auth { get; set; }

        public int? State { get; set; }
    }
}
