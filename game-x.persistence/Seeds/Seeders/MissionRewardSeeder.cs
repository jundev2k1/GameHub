using game_x.domain.Entities.Rewards;
using game_x.persistence.Seeds.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class MissionRewardSeeder : ISeeder
{
    public async Task SeedAsync(
        GameXContext context,
        CancellationToken ct = default)
    {
        var missionMap = await context.Missions
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Code, x => x.Id, ct);

        var rewardMap = await context.RewardDefinitions
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Code, x => x.Id, ct);

        var missionRewards = new List<MissionReward>();
        
        #region Daily Streak Login (7 days)
        if (missionMap.TryGetValue(MissionCode.DailyStreakLogin, out var dailyLoginId))
        {
            var dailyRewards = new[]
            {
                (RewardDefinitionCode.TicketX1, 1, 1, 1m),
                (RewardDefinitionCode.TicketX1, 2, 2, 2m),
                (RewardDefinitionCode.TicketX1, 3, 3, 3m),
                (RewardDefinitionCode.TicketX2, 4, 4, 4m),
                (RewardDefinitionCode.TicketX2, 5, 5, 5m),
                (RewardDefinitionCode.TicketX2, 6, 6, 6m),
                (RewardDefinitionCode.TicketX3, 7, 7, 7m)
            };

            foreach (var (rewardCode, sequence, sortOrder, progress) in dailyRewards)
            {
                if (!rewardMap.TryGetValue(rewardCode, out var rewardId))
                    continue;

                missionRewards.Add(
                    MissionReward.Create(
                        missionId: dailyLoginId,
                        rewardDefinitionId: rewardId,
                        sequence: sequence,
                        sortOrder: sortOrder,
                        requiredProgress: progress));
            }
        }
        #endregion

        #region Deposit
        AddSingleReward(
            MissionCode.FirstDeposit,
            RewardDefinitionCode.TicketX1,
            requiredProgress: 1);
        #endregion

        #region Deposit Accumulation (Multi-tier deposit ladder)
        if (missionMap.TryGetValue(MissionCode.DepositAccumulation, out var depositAccumId))
        {
            var depositRewards = new[]
            {
                (RewardDefinitionCode.TicketX1, 1, 1, 100m),
                (RewardDefinitionCode.TicketX2, 2, 2, 500m),
                (RewardDefinitionCode.TicketX3, 3, 3, 1000m)
            };

            foreach (var (rewardCode, sequence, sortOrder, progress) in depositRewards)
            {
                if (!rewardMap.TryGetValue(rewardCode, out var rewardId))
                    continue;

                missionRewards.Add(
                    MissionReward.Create(
                        missionId: depositAccumId,
                        rewardDefinitionId: rewardId,
                        sequence: sequence,
                        sortOrder: sortOrder,
                        requiredProgress: progress));
            }
        }
        #endregion

        #region Share (invite/share converts to signup)
        AddSingleReward(
            MissionCode.SocialShare,
            RewardDefinitionCode.TicketX1,
            requiredProgress: 1);
        
        AddSingleReward(
            MissionCode.ShareConversion,
            RewardDefinitionCode.TicketX2,
            requiredProgress: 1);
        #endregion

        foreach (var reward in missionRewards)
        {
            var exists = await context.MissionRewards
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MissionId == reward.MissionId &&
                    x.Sequence == reward.Sequence,
                    ct);

            if (exists)
                continue;

            await context.MissionRewards.AddAsync(reward, ct);
        }

        await context.SaveChangesAsync(ct);

        #region Local Helper
        void AddSingleReward(
            string missionCode,
            string rewardCode,
            decimal requiredProgress,
            bool isClaimable = true)
        {
            if (!missionMap.TryGetValue(missionCode, out var missionId))
                return;

            if (!rewardMap.TryGetValue(rewardCode, out var rewardId))
                return;

            missionRewards.Add(
                MissionReward.Create(
                    missionId: missionId,
                    rewardDefinitionId: rewardId,
                    sequence: 1,
                    sortOrder: 1,
                    requiredProgress: requiredProgress,
                    isClaimable: isClaimable));
        }
        #endregion
        
        await context.SaveChangesAsync(ct);
    }
}