using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.Missions.Claim;

public sealed record ClaimMissionRewardCommand(Guid ClaimId ) : ICommand<ClaimMissionRewardResponse>;

public sealed record ClaimMissionRewardResponse
{
    public Guid? RewardId { get; init; }
    public string? RewardCode { get; init; }
    public string? RewardTitle { get; init; }
    public decimal? Amount { get; init; }
    public RewardItemType? RewardType { get; init; }
    public bool? CycleCompleted { get; init; }
};