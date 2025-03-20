using System.Text.Json;

namespace coords;

public class LocationGenerator
{
    public static async Task Main()
    {
        var lonStep = 0.01;
        var latStep = 0.01;

        var filePath = "Locations.json";

        // Start the JSON file with an opening bracket
        File.WriteAllText(filePath, "[\n");

        var firstEntry = true;
        var i = 1;

        for (var lon = -180.0; lon <= 180.0; lon += lonStep)
        {
            for (var lat = -90.0; lat <= 90.0; lat += latStep)
            {
                var alt = 0;//await Elevation.GetElevation(lat, lon);
                var climateZone = ClimateZone.DetermineClimateZone(lat, lon, alt);
                var timezone = Timezone.DetermineTimezone(lat, lon);
                var surfaceType = SurfaceType.DetermineSurfaceType(lat, lon, alt);

                var location = $"{Math.Round(lon, 4)} {Math.Round(lat, 4)} {alt} {climateZone} {timezone} {surfaceType}";

                // var location = new Location
                // {
                //     Longitude = Math.Round(lon, 4),
                //     Latitude = Math.Round(lat, 4),
                //     Altitude = alt,
                //     ClimateZone = climateZone,
                //     Timezone = timezone,
                //     SurfaceType = surfaceType
                // };

                var json = JsonSerializer.Serialize(location, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                // Append JSON object to the file
                using (StreamWriter sw = new StreamWriter(filePath, append: true))
                {
                    if (!firstEntry)
                        sw.WriteLine(",");
                    sw.Write(json);
                }

                firstEntry = false;

                Console.WriteLine(i+") "+location);

                i++;
            }
        }

        using (StreamWriter sw = new StreamWriter(filePath, append: true))
        {
            sw.WriteLine("\n]");
        }

        Console.WriteLine("✅ JSON file 'Locations.json' has been updated successfully!");
    }
}