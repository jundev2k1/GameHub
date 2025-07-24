using System.Text.Json.Serialization;
using System.Text.Json;

namespace game_x.share.Extensions;

public static class ObjectExtension
{
    public static T DeepClone<T>(this T obj) where T : class
    {
        if (obj == null) return default!;

        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
