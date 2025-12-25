using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Converters;

public sealed class FlexibleDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetDecimal();

            case JsonTokenType.String:
            {
                var s = reader.GetString();

                if (string.IsNullOrWhiteSpace(s))
                    return 0m;

                if (decimal.TryParse(
                        s,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var value))
                {
                    return value;
                }

                break;
            }

            case JsonTokenType.Null:
                return 0m;
        }

        throw new JsonException(
            $"Cannot convert JSON token '{reader.TokenType}' to decimal.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        decimal value,
        JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}