using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkCreate;

public sealed record BulkCreateRewardPoolItemCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid RewardPoolId { get; init; }

    public IReadOnlyCollection<BulkCreateRewardPoolItemDto> Items { get; init; } = [];
}

public sealed record BulkCreateRewardPoolItemDto
{
    public Guid RewardDefinitionId { get; init; }

    public int Weight { get; init; }

    public int SortOrder { get; init; }

    public DateTime? StartAt { get; init; }

    public DateTime? EndAt { get; init; }
}