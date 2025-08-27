using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GameInfoDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string GameCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public GameCategoryInfo[] Categories { get; set; } = [];
    public GameTypeInfo[] GameTypes { get; set; } = [];
    [JsonIgnore]
    public int Priority { get; set; } = 0;
}

public sealed class GameCategoryInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    [JsonIgnore]
    public int Priority { get; set; }
}

public sealed class GameTypeInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    [JsonIgnore]
    public int Priority { get; set; }
}
