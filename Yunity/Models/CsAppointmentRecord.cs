using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class CsAppointmentRecord
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public string? CustomerName { get; set; }

    public string? Phone { get; set; }

    public string? ServiceLocation { get; set; }

    public string? Email { get; set; }

    public string? ArNotes { get; set; }

    public string? Feedback { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public DateTime? FinishDate { get; set; }
}
