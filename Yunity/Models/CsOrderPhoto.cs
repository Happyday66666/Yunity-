using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CsOrderPhoto
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public string? PhotoId { get; set; }
}
