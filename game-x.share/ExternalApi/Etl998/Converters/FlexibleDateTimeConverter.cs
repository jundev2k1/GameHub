using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Converters;
public sealed class FlexibleDateTimeConverter(DateTimeKind defaultKind = DateTimeKind.Local) : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dt = ReadNullable(ref reader, options);
        return dt ?? default;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write ISO 8601 (round-trip)
        writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
    }

    internal DateTime? ReadNullable(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();

            if (string.IsNullOrWhiteSpace(s))
                return null;

            s = s.Trim();

            // Some APIs return "0" or "0000-00-00..." as empty
            if (s == "0" || s.StartsWith("0000-00-00", StringComparison.Ordinal))
                return null;

            // 1) Try DateTimeOffset (handles Z / +07:00 etc.)
            if (DateTimeOffset.TryParse(
                    s,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                    out var dto))
            {
                // If string had offset/Z -> dto has it
                return ApplyDefaultKind(dto.DateTime);
            }

            // 2) Try exact formats commonly returned by ETL-like systems
            // Add/remove formats as you observe in logs
            var formats = new[]
            {
                "yyyy-MM-dd",
                "yyyy-M-d",
                "yyyy/MM/dd",
                "yyyy/M/d",

                "yyyy-MM-dd HH:mm:ss",
                "yyyy-M-d H:m:s",
                "yyyy-MM-dd HH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fffZ",

                "dd/MM/yyyy",
                "d/M/yyyy",
                "dd-MM-yyyy",
                "d-M-yyyy",

                "dd/MM/yyyy HH:mm:ss",
                "d/M/yyyy H:m:s",
            };

            if (DateTime.TryParseExact(
                    s,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out var dtExact))
            {
                return ApplyDefaultKind(dtExact);
            }

            // 3) Fallback: loose parse
            if (DateTime.TryParse(
                    s,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out var dtLoose))
            {
                return ApplyDefaultKind(dtLoose);
            }

            // Can't parse -> null (don't throw for ETL998)
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            // Epoch seconds or milliseconds
            if (!reader.TryGetInt64(out var n))
                return null;

            if (n <= 0)
                return null;

            // Heuristic: 13 digits ~ ms, 10 digits ~ seconds
            DateTimeOffset dto;
            if (n >= 1_000_000_000_000) // ms
                dto = DateTimeOffset.FromUnixTimeMilliseconds(n);
            else // seconds
                dto = DateTimeOffset.FromUnixTimeSeconds(n);

            // Convert to local/utc depending on preference
            var dt = defaultKind == DateTimeKind.Utc
                ? dto.UtcDateTime
                : dto.LocalDateTime;

            return dt;
        }

        // Unexpected token: skip safely
        using var _ = JsonDocument.ParseValue(ref reader);
        return null;
    }

    private DateTime ApplyDefaultKind(DateTime dt)
    {
        // If dt already has Kind set (rare from TryParseExact), keep it.
        // Otherwise, set to _defaultKind.
        if (dt.Kind != DateTimeKind.Unspecified)
            return dt;

        return defaultKind switch
        {
            DateTimeKind.Utc => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            DateTimeKind.Local => DateTime.SpecifyKind(dt, DateTimeKind.Local),
            _ => dt // Unspecified
        };
    }
}