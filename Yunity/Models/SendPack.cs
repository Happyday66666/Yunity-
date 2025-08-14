using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class SendPack
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ManagerId { get; set; }

    public int? Type { get; set; }

    public string? GetUser { get; set; }

    public string? GetTel { get; set; }

    public string? SendAddr { get; set; }

    public string? Logistic { get; set; }

    public string? PickUser { get; set; }

    public DateTime? PickTime { get; set; }

    public string? PackPhoto { get; set; }

    public DateTime? PickLogisticTime { get; set; }

    public int? State { get; set; }
}
