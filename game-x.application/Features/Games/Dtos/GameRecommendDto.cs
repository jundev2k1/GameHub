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
    public int Priority { get; set; } = 0;
    public string? CustomTitle { get; set; }
    [JsonIgnore]
    public bool IsActive { get; set; } = true;
    public bool IsGameActive { get; set; } = true;
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; }
}

public record GameRecommendListItemDto : GameItemDto
{
    public string? CustomTitle { get; init; }
    public int Priority { get; init; }
    public bool IsActive { get; init; }

    public GameRecommendListItemDto(GameInfoDto gameInfo, GameRecommendItemDto recommendItem)
        : base(gameInfo)
    {
        CustomTitle = recommendItem.CustomTitle;
        Priority = recommendItem.Priority;
        IsActive = recommendItem.IsActive;
    }
}
