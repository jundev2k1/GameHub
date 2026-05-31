using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GameCategoryDetailDto : GameCategoryDto
{
    public CategoryRelatedGameDto[] RelatedGames { get; set; } = [];
    public GameCategoryTranslationInfo[] GameTranslations => [.. CategoryTranslations.Values];
}

public sealed class CategoryRelatedGameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlatformName { get; set; } = string.Empty;
}

public sealed class GameCategoryTranslationInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    [JsonIgnore]
    public int CategoryId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
