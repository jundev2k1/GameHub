using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GameInfoDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string GameCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public GameCategoryInfo[] Categories { get; set; } = [];
    public GameTypeInfo[] GameTypes { get; set; } = [];
    public int Priority { get; set; } = 0;
    public bool IsActive { get; set; }
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
