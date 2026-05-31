using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Converters;

public sealed class FlexibleNullableDateTimeConverter(DateTimeKind defaultKind = DateTimeKind.Local) : JsonConverter<DateTime?>
{
    private readonly FlexibleDateTimeConverter _inner = new(defaultKind);


    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => _inner.ReadNullable(ref reader, options);

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
    }
}