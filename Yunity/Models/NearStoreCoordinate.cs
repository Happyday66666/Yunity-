using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class NearStoreCoordinate
{
    public int NearStoreCoordinatesId { get; set; }

    public int? NearStoreId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
