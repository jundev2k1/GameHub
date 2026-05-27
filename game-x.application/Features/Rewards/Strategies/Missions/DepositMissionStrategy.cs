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
        var today = userEvent.CreatedAt.Date;
        if (userMission.Status != UserMissionStatus.InProgress) return;
        
        var config = mission.ConfigData;
        
        if (userEvent.Value >= config.MinimumValue)
        {
            var value = config.ProgressMode == MissionProgressMode.Count ? 1 : userEvent.Value ?? 0;
            userMission.AddProgress((int)value, today, false);
        }

        if (userMission.Progress >= config.RequiredProgress)
        {
            var rewards = await missionRewardRepo.GetByMissionIdAsync(mission.Id, ct);
            var unlockedRewards = rewards
                .Where(x => userMission.Progress >= x.RequiredProgress)
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
            
            if(mission.ResetType == MissionResetType.Never)
                userMission.Complete();
        }
    }
}