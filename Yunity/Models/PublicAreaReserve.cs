using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class PublicAreaReserve
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public string? Name { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public int? DeductUnit { get; set; }

    public string? UseTime { get; set; }

    public string? AreaInfo { get; set; }

    public int? Amount { get; set; }

    public string? Photo { get; set; }

    public int? Icon { get; set; }

    public decimal? UseTimeUnit { get; set; }
}
