using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.Create;

public sealed record CreateRewardPoolItemCommand: ICommand<Unit>
{
    [JsonIgnore]
    public Guid RewardPoolId { get; init; }
    public Guid RewardDefinitionId { get; init; }
    public int Weight { get; init; }
    public int SortOrder { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}