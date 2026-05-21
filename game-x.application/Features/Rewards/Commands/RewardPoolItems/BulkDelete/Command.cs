using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkDelete;

public sealed record BulkDeleteRewardPoolItemCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid RewardPoolId { get; init; }

    public IReadOnlyCollection<Guid> ItemIds { get; init; } = [];
}