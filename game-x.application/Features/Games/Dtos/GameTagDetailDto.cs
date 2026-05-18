using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GameTagDetailDto : GameTagDto
{
    public GameTagRelatedGameDto[] RelatedGames { get; set; } = [];
    public GameTagTranslationInfo[] Translations => [.. TagTranslations.Values];
}

public sealed class GameTagRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlatformName { get; set; } = string.Empty;
}

public sealed class GameTagTranslationInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    [JsonIgnore]
    public int TagId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
