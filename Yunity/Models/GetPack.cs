using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class GetPack
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public string? PackNo { get; set; }

    public int Type { get; set; }

    public int? DoorNoId { get; set; }

    public int? ManagerId { get; set; }

    public string? GetUser { get; set; }

    public string? Logistic { get; set; }

    public DateTime? LogTime { get; set; }

    public string? PackPhoto { get; set; }

    public string? PickUser { get; set; }

    public DateTime? PickTime { get; set; }

    public int? State { get; set; }
}
