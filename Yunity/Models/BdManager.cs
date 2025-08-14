using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class BdManager
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public int? ManagerId { get; set; }

    public int? Permissions { get; set; }
}
