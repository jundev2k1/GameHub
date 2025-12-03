using game_x.share.Extensions;
using Newtonsoft.Json;
using System.Text.Json;

namespace game_x.share.Helper;

public static class JsonHelper
{
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
}