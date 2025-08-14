using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CsProduct
{
    public int Id { get; set; }

    public int? ComId { get; set; }

    public string? PCategory { get; set; }

    public string? PName { get; set; }

    public string? PDescription { get; set; }

    public decimal? PPrice { get; set; }

    public int? PStock { get; set; }

    public string? PStatus { get; set; }
}
