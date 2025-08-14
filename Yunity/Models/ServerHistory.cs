using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class ServerHistory
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public DateTime? ServiceDate { get; set; }

    public string? ServiceType { get; set; }

    public string? ServiceWho { get; set; }

    public string? ServiceStatus { get; set; }

    public string? ServiceFeedback { get; set; }
}
