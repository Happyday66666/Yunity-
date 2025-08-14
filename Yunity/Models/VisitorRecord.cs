using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class VisitorRecord
{
    public int Id { get; set; }

    public int? DoorNoId { get; set; }

    public int? ManagerId { get; set; }

    public string? VisitorName { get; set; }

    public DateTime? VisitTime { get; set; }

    public string? VisitReason { get; set; }

    public int? BdId { get; set; }
}
