using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coords;

public class DepthCalculator
{
    public static double DetermineDepth(double lat, double lon, string surfaceType)
    {
        var random = new Random();

        return surfaceType switch
        {
            // Ocean depths range from 200m (continental shelf) to 11,000m (Mariana Trench)
            "OCN" => Math.Round(random.Next(200, 11000) + random.NextDouble(), 2),
            // Sea depths range from 10m (shallow coastal) to 3000m (deepest seas)
            "SEA" => Math.Round(random.Next(10, 3000) + random.NextDouble(), 2),
            // Deltas are generally shallow, ranging from 0m to 50m
            "DLT" => Math.Round(random.Next(0, 50) + random.NextDouble(), 2),
            // Rivers are mostly shallow, usually 1m to 50m depth
            "RIV" => Math.Round(random.Next(1, 50) + random.NextDouble(), 2),
            // Coastal areas range from 0m (beach) to 200m (continental shelf)
            "CST" => Math.Round(random.Next(0, 200) + random.NextDouble(), 2),
            // Default for land, assuming no depth
            _ => 0,
        };
    }
}