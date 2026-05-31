namespace game_x.share.ExternalApi.Etl998.Converters;

using System.Globalization;

public static class EtlDateTimeConverter
{
    private static readonly TimeZoneInfo EtlTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "China Standard Time"
                : "Asia/Shanghai");

    public static DateTime ToUtc(string etlDateTime)
    {
        var localTime = DateTime.ParseExact(
            etlDateTime,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None);
        var etlTime = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(etlTime, EtlTimeZone);
    }
    
    public static string ToEtlDate(DateTime utc)
    {
        return utc.ToString("yyyy-M-d", CultureInfo.InvariantCulture);
    }
}