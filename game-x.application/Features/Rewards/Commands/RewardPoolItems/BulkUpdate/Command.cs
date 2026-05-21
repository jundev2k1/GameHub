using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkUpdate;

public sealed record BulkUpdateRewardPoolItemCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid RewardPoolId { get; init; }

    public IReadOnlyCollection<BulkUpdateRewardPoolItemDto> Items { get; init; } = [];
}

public sealed record BulkUpdateRewardPoolItemDto
{
    public Guid Id { get; init; }

    public Guid? RewardDefinitionId { get; init; }

    public int? Weight { get; init; }

    public int? SortOrder { get; init; }

    public bool? IsActive { get; init; }

    public DateTime? StartAt { get; init; }

    public DateTime? EndAt { get; init; }
}