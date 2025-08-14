using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class NearStore
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? OpenTime { get; set; }

    public string? Addr { get; set; }

    public int? Type { get; set; }

    public string? Photo { get; set; }

    public string? Info { get; set; }

    public DateTime UpdateTime { get; set; }

    public string? NsPhone { get; set; }
}
