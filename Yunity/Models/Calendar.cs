using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class Calendar
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public int? ManagerId { get; set; }

    public DateTime? EventStart { get; set; }

    public string? EventName { get; set; }

    public string? Info { get; set; }

    public DateTime? LogTime { get; set; }

    public DateTime? EventEnd { get; set; }
}
