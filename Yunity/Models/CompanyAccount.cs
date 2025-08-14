using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CompanyAccount
{
    public int Id { get; set; }

    public string? ComName { get; set; }

    public string? ComAccount { get; set; }

    public string? ComPassword { get; set; }

    public int? ComPermissions { get; set; }

    public int? ComStatus { get; set; }

    public string FAspUserId { get; set; } = null!;
}
