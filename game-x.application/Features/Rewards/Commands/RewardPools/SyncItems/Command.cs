using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.RewardPools.SyncItems;

public sealed record SyncRewardPoolItemCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid RewardPoolId { get; init; }

    public IReadOnlyCollection<CreateRewardPoolItemDto> CreatedItems { get; init; } = [];

    public IReadOnlyCollection<UpdateRewardPoolItemDto> UpdatedItems { get; init; } = [];

    public IReadOnlyCollection<Guid> DeletedItems { get; init; } = [];
}

public sealed record CreateRewardPoolItemDto
{
    public Guid RewardDefinitionId { get; init; }

    public int Weight { get; init; }

    public int SortOrder { get; init; }

    public DateTime? StartAt { get; init; }

    public DateTime? EndAt { get; init; }
}

public sealed record UpdateRewardPoolItemDto
{
    public Guid Id { get; init; }

    public Guid? RewardDefinitionId { get; init; }

    public int? Weight { get; init; }

    public int? SortOrder { get; init; }

    public bool? IsActive { get; init; }

    public DateTime? StartAt { get; init; }

    public DateTime? EndAt { get; init; }
}