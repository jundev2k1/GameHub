using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Strategies.Missions;

public sealed class DepositMissionStrategy(
    IMissionRewardRepo missionRewardRepo,
    IUserMissionClaimRepo userMissionClaimRepo): IMissionProgressStrategy
{
    public UserEventType SupportedType => UserEventType.DepositCompleted;
    
    public async Task ProcessAsync(
        Mission mission,
        UserMission userMission,
        UserEvent userEvent,
        CancellationToken ct = default)
    {
        var config = mission.ConfigData;
        var today = userEvent.CreatedAt.Date;

        if (userMission.HasProgressToday(today) || userMission.InvalidTime(today)) return;

        // Expire all previous rewards
        await userMissionClaimRepo.ExpireUnclaimedAsync(userMission.Id, userMission.CycleNumber, ct);
        
        if (config.RequireConsecutiveProgress && 
            (userMission.IsMissedRequiredDay(today) || userMission.Progress >= config.RequiredProgress))
        {
            userMission.ResetProgress();
        }
        
        var consecutive = userMission.LastProgressAt?.Date == today.AddDays(-1);
        userMission.AddProgress(today, consecutive);

        var rewards = await missionRewardRepo.GetByMissionIdAsync(mission.Id, ct);
        var unlockedRewards = rewards
            .Where(x => x.Sequence == userMission.Progress)
            .ToArray();

        foreach (var reward in unlockedRewards)
        {
            var exists =
                await userMissionClaimRepo.ExistsAsync(
                    userMissionId: userMission.Id,
                    missionRewardId: reward.Id,
                    cycleNumber: userMission.CycleNumber,
                    ct);

            if (exists) continue;

            var claim = UserMissionClaim.Create(
                userId: userEvent.UserId,
                userMission: userMission,
                missionRewardId: reward.Id,
                cycleNumber: userMission.CycleNumber);

            await userMissionClaimRepo.AddAsync(claim, ct);
        }
    }
}