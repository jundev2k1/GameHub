using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.Missions.SyncItems;

public sealed record SyncMissionRewardCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid MissionId { get; init; }

    public IReadOnlyCollection<CreateMissionRewardItem> CreatedItems { get; init; } = [];

    public IReadOnlyCollection<UpdateMissionRewardItem> UpdatedItems { get; init; } = [];

    public IReadOnlyCollection<Guid> DeletedItems { get; init; } = [];
}

public sealed record CreateMissionRewardItem
{
    public Guid RewardDefinitionId { get; init; }
    public int Sequence { get; init; }
    public int SortOrder { get; init; }
    public decimal RequiredProgress { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}

public sealed record UpdateMissionRewardItem
{
    public Guid Id { get; init; }
    public Guid RewardDefinitionId { get; init; }
    public int Sequence { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public int RequiredProgress { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}