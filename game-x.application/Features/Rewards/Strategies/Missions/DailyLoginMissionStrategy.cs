using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Strategies.Missions;

public sealed class DailyLoginMissionStrategy(
    IMissionRewardRepo missionRewardRepo, 
    IUserMissionClaimRepo userMissionClaimRepo) : IMissionProgressStrategy
{
    public MissionType SupportedType => MissionType.DailyLogin;

    public async Task ProcessAsync(Mission mission, UserMission userMission, UserEvent userEvent, CancellationToken ct = default)
    {
        var config = mission.ConfigData;
        var today = DateTime.UtcNow.Date;

        if (userMission.HasProgressToday(today)) return;

        var consecutive = false;

        if (userMission.LastProgressAt.HasValue)
        {
            var yesterday = today.AddDays(-1);

            if (userMission.LastProgressAt.Value.Date == yesterday)
                consecutive = true;
            else if (config is {RequireConsecutiveProgress: true, ResetProgressOnMiss: true})
                userMission.ResetProgress();
        }

        userMission.AddProgress(today, consecutive);

        if (userMission.Progress >= config.RequiredProgress) userMission.Complete();
        
        var rewards = await missionRewardRepo.GetByMissionIdAsync(mission.Id, ct);

        var unlockedRewards = rewards
            .Where(x => x.Sequence == userMission.Progress)
            .ToArray();

        foreach (var reward in unlockedRewards)
        {
            var exists = await userMissionClaimRepo.CheckExistAsync(userEvent.UserId, reward.Id, ct);
            if (exists) continue;
  
            var claim = UserMissionClaim.Create(
                userId: userEvent.UserId,
                userMission: userMission,
                missionRewardId: reward.Id);

            await userMissionClaimRepo.AddAsync(claim, ct);
        }
    }
}