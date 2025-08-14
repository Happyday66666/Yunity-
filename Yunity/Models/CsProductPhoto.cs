using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CsProductPhoto
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? PhotoId { get; set; }
}
