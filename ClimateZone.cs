namespace coords;

public class ClimateZone
{
    public static string DetermineClimateZone(double lat, double lon, double elevation)
    {
        // High-altitude areas (Tundra)
        if (elevation > 2500)
            return "ET";

        // Tropical climates
        if (lat >= -23.5 && lat <= 23.5)
            return elevation < 1000 ? "Af" : "As"; // Rainforest or Tropical Wet & Dry

        // Hot Deserts
        if ((lat > 15 && lat < 35) &&
            ((lon > -120 && lon < -30) || (lon > 30 && lon < 120) || (lon > -70 && lon < -50)))
            return "BWh"; // Hot Desert

        // Cold Deserts
        if ((lat > 35 && lat < 50) &&
            ((lon > 40 && lon < 120) || (lon > -120 && lon < -100)))
            return "BWk"; // Cold Desert

        // Temperate zones
        if (lat >= 35 && lat < 50)
        {
            if ((lon > -125 && lon < -60) || (lon > 0 && lon < 30))
                return "Cfb"; // Marine West Coast
            return "Cfa"; // Humid Subtropical
        }

        // Boreal/Cold climates
        if (lat >= 50 && lat < 66.5)
        {
            if (Math.Abs(lon) < 30)
                return "Cfc"; // Subpolar Oceanic
            if ((lon > 60 && lon < 150) || (lon > -130 && lon < -60)) // Siberia, Canada
                return "Dfd"; // Very Cold Subarctic
            return "Dfc"; // Subarctic
        }

        // Ice Cap for polar regions
        return "EF";
    }
}