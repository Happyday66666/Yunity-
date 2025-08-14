using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class ManageFee
{
    public int Id { get; set; }

    public string? FeeName { get; set; }

    public int? DoorNoId { get; set; }

    public int? Price { get; set; }

    public int? MotorPrice { get; set; }

    public int? CarPrice { get; set; }

    public DateTime? FeeEnd { get; set; }

    public int? PayType { get; set; }

    public DateTime? PayTime { get; set; }

    public int? State { get; set; }

    public DateTime? LogTime { get; set; }

    public string? MerchantTradeNo { get; set; }
}
