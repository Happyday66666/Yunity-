using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class VendorCoordinate
{
    public int VendorCoordinatesId { get; set; }

    public int? CompanyProfileId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
