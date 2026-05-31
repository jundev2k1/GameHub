using System.Text.Json;
using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Converters;

public sealed class Etl998ResultConverter<T> : JsonConverter<T?>
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new FlexibleDecimalConverter(),
        }
    };
    
    public override T? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.String => default,
            JsonTokenType.StartObject
                => JsonSerializer.Deserialize<T>(
                    ref reader,
                    _options),

            _ => SkipAndReturnDefault(ref reader)
        };
    }

    private static T? SkipAndReturnDefault(ref Utf8JsonReader reader)
    {
        using var _ = JsonDocument.ParseValue(ref reader);
        return default;
    }

    public override void Write(
        Utf8JsonWriter writer,
        T? value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(
            writer,
            value,
            _options);
    }
}