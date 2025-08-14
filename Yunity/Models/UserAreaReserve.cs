using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class UserAreaReserve
{
    public int Id { get; set; }

    public int? AreaId { get; set; }

    public int? DoorNoId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ReserveTime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? State { get; set; }

    public int? ReservedPeople { get; set; }
}
