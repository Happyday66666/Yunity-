using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class BdList
{
    public int Id { get; set; }

    public string? BdNo { get; set; }

    public string? BdName { get; set; }

    public string? BdAddr { get; set; }

    public DateTime? ContractStart { get; set; }

    public DateTime? ContractEnd { get; set; }

    public string? FirstContact { get; set; }

    public string? FstContactTel { get; set; }

    public string? SecondContact { get; set; }

    public string? SecContactTel { get; set; }

    public string? HouseCount { get; set; }

    public int? BdState { get; set; }
}
