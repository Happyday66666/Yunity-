using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class Board
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public int? ManagerId { get; set; }

    public int Type { get; set; }

    public string? Name { get; set; }

    public string? Info { get; set; }

    public string? Photo { get; set; }

    public DateTime? OpenTime { get; set; }

    public int? Auth { get; set; }

    public int State { get; set; }
}
