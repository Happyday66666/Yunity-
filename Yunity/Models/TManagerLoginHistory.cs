using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class TManagerLoginHistory
{
    public int Id { get; set; }

    public int FManId { get; set; }

    public DateTime? FLoginTime { get; set; }

    public DateTime? FLogoutTime { get; set; }

    public string FIpAddress { get; set; } = null!;

    public string FLoginStatus { get; set; } = null!;
}
