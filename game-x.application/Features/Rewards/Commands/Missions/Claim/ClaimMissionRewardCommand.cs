using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.Missions.Claim;

public sealed record ClaimMissionRewardCommand(
    Guid MissionId,
    Guid ClaimId
    // string? IdempotencyKey
) : ICommand<ClaimMissionRewardResponse>;

public sealed record ClaimMissionRewardResponse
{
    public Guid? ExecutionId { get; init; }
    public Guid? UserRewardId { get; init; }
    public string? RewardTitle { get; init; }
    public decimal? Amount { get; init; }
    public RewardItemType? RewardType { get; init; }
};