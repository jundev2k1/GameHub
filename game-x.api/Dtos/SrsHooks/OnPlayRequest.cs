using game_x.api.Common;
using System.Text.Json.Serialization;

namespace game_x.api.Dtos.SrsHooks;

public class OnPlayRequest : SrsEventHookRequest
{
    [JsonPropertyName("pageUrl")]
    public string PageUrl { get; set; } = string.Empty;
}
