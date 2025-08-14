using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class TusersInfo
{
    public int FId { get; set; }

    public string FName { get; set; } = null!;

    public string FAccount { get; set; } = null!;

    public string FPassword { get; set; } = null!;

    public string FPhone { get; set; } = null!;

    public int FBuildingId { get; set; }

    public string FUserAddress { get; set; } = null!;

    public string FEmail { get; set; } = null!;

    public string FAspUserId { get; set; } = null!;

    public byte[]? QrCord { get; set; }
}
