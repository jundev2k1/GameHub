using Newtonsoft.Json;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.Update;

public sealed record UpdateRewardPoolItemCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public Guid? RewardDefinitionId { get; init; }
    public int? Weight { get; init; }
    public int? SortOrder { get; init; }
    public bool? IsActive { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}