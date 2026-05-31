using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GamePlatformDetailDto : GamePlatformDto
{
    public PlatformRelatedGameDto[] RelatedGames { get; set; } = [];
    public GamePlatformTranslationInfo[] Translations => [.. PlatformTranslations.Values];
}

public sealed class PlatformRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class GamePlatformTranslationInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    [JsonIgnore]
    public int PlatformId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
