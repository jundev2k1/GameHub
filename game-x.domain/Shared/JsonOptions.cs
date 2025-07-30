using System.Text.Encodings.Web;
using System.Text.Json;

namespace game_x.domain.Shared;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions NoEscape = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}