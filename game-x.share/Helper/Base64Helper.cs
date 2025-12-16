using System.Text;
using System.Text.Json;

namespace game_x.share.Helper;

public static class Base64Helper
{
    public static string Encode<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    public static T Decode<T>(string token)
    {
        var bytes = Convert.FromBase64String(token);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
