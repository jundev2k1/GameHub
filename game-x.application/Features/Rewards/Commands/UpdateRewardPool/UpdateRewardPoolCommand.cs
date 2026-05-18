using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;
using Newtonsoft.Json;

namespace game_x.application.Features.Rewards.Commands.UpdateRewardPool;

public sealed record UpdateRewardPoolCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public RewardPoolType? Type { get; init; }
    public bool? IsActive { get; init; }
    public int? SortOrder { get; init; }
    public RewardPoolConfigData? Config { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}