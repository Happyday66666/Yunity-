using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class DoorNo
{
    public int Id { get; set; }

    public int? BdId { get; set; }

    public string? DoorNo1 { get; set; }

    public decimal? SquareFeet { get; set; }

    public int? Points { get; set; }

    public int? MotorPark { get; set; }

    public int? CarPark { get; set; }
}
