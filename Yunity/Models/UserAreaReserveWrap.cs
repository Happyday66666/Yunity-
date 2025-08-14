using Yunity.Areas.Identity.Data;

namespace Yunity.Models
{
    internal class UserAreaReserveWrap
    {
        public YunityUser User { get; set; }
        public List<PublicAreaReserve> PublicAreaReserves { get; set; }
        public List<UserAreaReserveWithAreaInfo> UserAreaReserves { get; set; }
        public int UserPoints { get; set; }
    }
}