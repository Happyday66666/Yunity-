using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class NearStoreWithBd
{
    public int NearStoreWithBdId { get; set; }

    public int? NearStoreId { get; set; }

    public int? BdId { get; set; }

    public int? State { get; set; }
}
