using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CsPhoto
{
    public int Id { get; set; }

    public string? PhotoId { get; set; }

    public string? PhotoPath { get; set; }

    public DateTime? UploadTime { get; set; }
}
