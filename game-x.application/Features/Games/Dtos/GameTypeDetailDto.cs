using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GameTypeDetailDto : GameTypeDto
{
    public GameTypeRelatedGameDto[] RelatedGames { get; set; } = [];
    public GameTypeTranslationInfo[] Translations => [.. TypeTranslations.Values];
}

public sealed class GameTypeRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlatformName { get; set; } = string.Empty;
}

public sealed class GameTypeTranslationInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    [JsonIgnore]
    public int TypeId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
