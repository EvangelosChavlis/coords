using System.Text.Json;

namespace coords;

public class Elevation
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<double> GetElevation(double lat, double lon)
    {
        var tempClimateZone = ClimateZone.DetermineClimateZone(lat, lon, 0);
        var tempSurfaceType = SurfaceType.DetermineSurfaceType(lat, lon, 0);

         // Skip elevation lookup for polar, coastal regions, certain climate zones, oceans, seas, and rivers
        if (tempSurfaceType == "POL" || tempSurfaceType == "CST" || 
            tempClimateZone == "Cfc" || tempClimateZone == "Cfb" ||
            tempSurfaceType == "OCN" || tempSurfaceType == "SEA")
        {
            return 0.0;
        }

        try
        {
            var url = $"https://api.open-elevation.com/api/v1/lookup?locations={lat},{lon}";
            var response = await client.GetStringAsync(url);
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
            var elevation = jsonResponse.GetProperty("results")[0].GetProperty("elevation").GetDouble();
            
            await Task.Delay(1000);
            
            return elevation;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching elevation for ({lat}, {lon}): {ex.Message}");
            return 0;
        }
    }

}