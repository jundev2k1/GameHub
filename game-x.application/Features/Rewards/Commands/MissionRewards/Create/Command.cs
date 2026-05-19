using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.MissionRewards.Create;

public sealed record CreateMissionRewardCommand: ICommand<Unit>
{
    [JsonIgnore]
    public Guid MissionId { get; init; }
    public Guid RewardDefinitionId { get; init; }
    public int Sequence { get; init; }
    public int SortOrder { get; init; }
    public decimal RequiredProgress { get; init; }
    public bool IsClaimable { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}