using GeoTimeZone;

namespace coords;

public static class Timezone
{
    public static string DetermineTimezone(double latitude, double longitude)
    {
        try
        {
            var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
            if (string.IsNullOrEmpty(timeZoneId))
                return "N_A";

            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var offset = timeZoneInfo.BaseUtcOffset;

                // Format the offset with hours and minutes (e.g., UTC+12:00 or UTC-05:00)
                var sign = offset.TotalHours >= 0 ? "+" : "-";
                return $"UTC{sign}{Math.Abs(offset.Hours):D2}:{Math.Abs(offset.Minutes):D2}";
            }
            catch (TimeZoneNotFoundException)
            {
                return "N_A";
            }
        }
        catch (Exception)
        {
            return "N_A";
        }
    }
}