using System.Text.Json;
using game_x.share.Extensions;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace game_x.share.Helper;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public static bool IsJson(string input)
    {
        if (input.IsNullOrWhiteSpace())
            return false;

        try
        {
            using var doc = JsonDocument.Parse(input);
            return doc.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsJsonArray(string input)
    {
        if (input.IsNullOrWhiteSpace())
            return false;

        try
        {
            using var doc = JsonDocument.Parse(input);
            return doc.RootElement.ValueKind == JsonValueKind.Array;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryParseJson<T>(string json, out T? result) where T : class
    {
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };

        try
        {
            result = JsonConvert.DeserializeObject<T>(json, settings);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
    
    public static T ConvertJson<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOpts)
                   ?? throw new InvalidOperationException("Empty JSON payload");
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse JSON: {json}", ex);
        }
    }
}