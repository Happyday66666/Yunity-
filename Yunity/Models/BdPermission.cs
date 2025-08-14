using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class BdPermission
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public int? ReceivePackage { get; set; }

    public int? SentPackage { get; set; }

    public int? ManageFee { get; set; }

    public int? Bulletin { get; set; }

    public int? PublicAreaReserve { get; set; }

    public int? NearbyStore { get; set; }

    public int? CommunityService { get; set; }

    public int? Feedback { get; set; }

    public int? VisitorRecord { get; set; }

    public int? Calendar { get; set; }
}
