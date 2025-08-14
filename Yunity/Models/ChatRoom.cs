using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class ChatRoom
{
    public int Id { get; set; }

    public string? RoomId { get; set; }

    public string? UserId { get; set; }

    public string? MangerId { get; set; }

    public string? MsgText { get; set; }

    public DateTime? MsgTime { get; set; }
}
