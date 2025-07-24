using game_x.share.Extensions;
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
}
