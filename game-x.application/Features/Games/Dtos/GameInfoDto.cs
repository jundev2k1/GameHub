using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public class GameInfoDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string GameCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public ThumbnailInfo? Thumbnail { get; set; }
    public Guid PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public GameCategoryInfo[] Categories { get; set; } = [];
    public GameTypeInfo[] GameTypes { get; set; } = [];
    public GameTagInfo[] GameTags { get; set; } = [];
    public int Priority { get; set; } = 0;
    public bool IsActive { get; set; }
    [JsonIgnore]
    public Dictionary<string, GameTranslationInfo> Translations { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class ThumbnailInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    [JsonIgnore]
    public string BucketName { get; set; } = string.Empty;
    [JsonIgnore]
    public string ObjectName { get; set; } = string.Empty;
}

public sealed class GameCategoryInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
}

public sealed class GameTypeInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
}

public sealed class GameTagInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
}

public sealed class GameTranslationInfo
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public int GameId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}