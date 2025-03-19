using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GeoTimeZone;

public class Location
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double Altitude { get; set; }
    public string ClimateZone { get; set; } = "N_A"; 
    public string Timezone { get; set; }  = "N_A";
    public string SurfaceType { get; set; } = "N_A";
}

public class LocationGenerator
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main()
    {
        double lonStep = 0.0001;  
        double latStep = 0.0001;

        string filePath = "Locations.json";
    
        // Start the JSON file with an opening bracket
        File.WriteAllText(filePath, "[\n");

        bool firstEntry = true;

        var locations = new List<Location>();

        for (var lon = -180.0; lon <= 180.0; lon += lonStep)
        {
            for (var lat = -90.0; lat <= 90.0; lat += latStep)
            {
                var alt = await GetElevation(lat, lon);
                var climateZone = DetermineClimateZone(lat, lon, alt);
                var timezone = DetermineTimezone(lat, lon);
                var surfaceType = DetermineSurfaceType(lat, lon, alt);

                var location = new Location
                {
                    Longitude = Math.Round(lon, 4),
                    Latitude = Math.Round(lat, 4),
                    Altitude = alt,
                    ClimateZone = climateZone,
                    Timezone = timezone!,
                    SurfaceType = surfaceType
                };

                string json = JsonSerializer.Serialize(location, new JsonSerializerOptions { WriteIndented = false });

                // Append JSON object to the file
                using (StreamWriter sw = new StreamWriter(filePath, append: true))
                {
                    if (!firstEntry)
                    {
                        sw.WriteLine(",");
                    }
                    sw.Write(json);
                }

                firstEntry = false;

                Console.WriteLine($"({lon}, {lat}, {alt}, {climateZone}, {timezone}, {surfaceType})");

                await Task.Delay(1000);

              
            }
        }

        using (StreamWriter sw = new StreamWriter(filePath, append: true))
        {
            sw.WriteLine("\n]");
        }

        Console.WriteLine("✅ JSON file 'Locations.json' has been updated successfully!");
    }

    public static async Task<double> GetElevation(double lat, double lon)
    {
        try
        {
            var url = $"https://api.open-elevation.com/api/v1/lookup?locations={lat},{lon}";
            var response = await client.GetStringAsync(url);
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
            var elevation = jsonResponse.GetProperty("results")[0].GetProperty("elevation").GetDouble();
            return elevation;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching elevation for ({lat}, {lon}): {ex.Message}");
            return 0;
        }
    }

    public static string? DetermineTimezone(double latitude, double longitude)
    {
        var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var offset = timeZoneInfo.BaseUtcOffset;
        var utcOffsetString = $"UTC{(offset.TotalHours >= 0 ? "+" : "")}{offset.TotalHours}";
        
        if (utcOffsetString is null)
            return null;
            
        return utcOffsetString;
    }

    public static string DetermineClimateZone(double lat, double lon, double elevation)
    {
        if (elevation > 2500)
            return "ET"; // High-altitude areas turn into Tundra

        if (lat >= -23.5 && lat <= 23.5)
            return elevation < 1000 ? "Af" : "As"; // Tropical Rainforest or Tropical Wet & Dry

        if (lat > 23.5 && lat < 35)
        {
            if (Math.Abs(lon) > 100) // More inland regions are typically more arid
                return "BWh"; // Hot Desert
            return "BWk"; // Cold Desert (e.g., Atacama, parts of China)
        }

        if (lat >= 35 && lat < 50)
        {
            if (Math.Abs(lon) < 30) // Closer to oceans
                return "Cfb"; // Marine West Coast
            return "Cfa"; // Humid Subtropical
        }

        if (lat >= 50 && lat < 66.5)
        {
            if (Math.Abs(lon) < 30)
                return "Cfc"; // Subpolar Oceanic
            return "Dfb"; // Humid Continental
        }

        return "EF"; // Ice Cap for polar regions
    }

    public static string DetermineSurfaceType(double lat, double lon, double elevation)
    {
        // High-altitude areas
        if (elevation > 2500)
            return "MNT"; // Mountainous

        // Polar regions
        if (lat > 66.5 || lat < -66.5)
            return "POL"; // Polar

        // Rainforest and tropical regions
        if (lat >= -23.5 && lat <= 23.5)
            return elevation < 1000 ? "RFR" : "SVN"; // Rainforest or Savanna

        // Desert classification (based on general desert locations)
        if ((lat > 20 && lat < 35) && (lon > -120 && lon < 120)) // Sahara, Gobi, etc.
            return "DST"; // Desert

        // Steppe (semi-arid grasslands)
        if (lat >= 35 && lat < 50)
            return "STP"; // Steppe

        // Grasslands and forests (general rule)
        if (lat >= 50 && lat < 66.5)
            return elevation < 500 ? "GRS" : "FOR"; // Grassland or Forest

        // Coastal regions (approximate coastline proximity check)
        if (Math.Abs(lon) < 30 || Math.Abs(lon) > 150)
            return "CST"; // Coastal

        // Check if it's a Delta region (simplified logic)
        if ((lat > 10 && lat < 30) && (lon > 80 && lon < 100)) // Example: Ganges, Mekong
            return "DLT"; // Delta

        // Volcanic regions (simplified check for known locations)
        if ((lat > -5 && lat < 5) && (lon > 90 && lon < 130)) // Indonesia region
            return "VOL"; // Volcanic

        return "N_A"; // Default if no other match
    }

}