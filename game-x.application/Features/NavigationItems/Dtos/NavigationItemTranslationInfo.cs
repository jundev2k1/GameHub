using System.Text.Json.Serialization;

namespace game_x.application.Features.NavigationItems.Dtos;

public sealed class NavigationItemTranslationInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    [JsonIgnore]
    public int NavigationId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}
