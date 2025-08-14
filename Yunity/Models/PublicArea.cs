using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class PublicArea
{
    public int Id { get; set; }

    public int? PaId { get; set; }

    public string? PaName { get; set; }

    public string? Icont { get; set; }
}
