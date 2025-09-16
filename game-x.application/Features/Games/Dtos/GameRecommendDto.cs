using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public class GameRecommendDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? BannerId { get; set; }
    public PublishStatus Status { get; set; } = PublishStatus.Draft;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public GameRecommendItemDto[] Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class GameRecommendItemDto
{
    [JsonIgnore]
    public int LocalGameId { get; set; }
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    [JsonIgnore]
    public int LocalPlatformId { get; set; }
    public Guid PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public int Priority { get; set; } = 0;
    public string? CustomTitle { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}