using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coords;

public class Location
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double Altitude { get; set; }
    public string ClimateZone { get; set; } = "N_A";
    public string Timezone { get; set; } = "N_A";
    public string SurfaceType { get; set; } = "N_A";
}

