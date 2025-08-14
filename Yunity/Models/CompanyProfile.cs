using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CompanyProfile
{
    public int Id { get; set; }

    public int? ComId { get; set; }

    public string? ComAddress { get; set; }

    public string? ComPerson { get; set; }

    public string? ComPhone { get; set; }

    public string? ComEmail { get; set; }

    public string? ComServerItem { get; set; }

    public int? ComRegistrationNumber { get; set; }

    public DateTime? ComContractStartDate { get; set; }

    public DateTime? ComContractEndDate { get; set; }

    public string? ComBusinessHours { get; set; }

    public DateTime? ComModifyTime { get; set; }
}
