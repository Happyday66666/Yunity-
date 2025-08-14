using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class BdMember
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public string? DoorNo { get; set; }

    public int? UserId { get; set; }

    public int? DoorNoId { get; set; }
}
