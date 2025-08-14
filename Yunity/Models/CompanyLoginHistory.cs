using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CompanyLoginHistory
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public DateTime? ComLoginTime { get; set; }

    public DateTime? ComLogoutTime { get; set; }

    public string? ComIpAddress { get; set; }

    public string? ComLoginStatus { get; set; }
}
