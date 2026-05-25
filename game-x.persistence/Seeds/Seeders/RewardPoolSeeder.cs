using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;
using game_x.persistence.Seeds.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class RewardPoolSeeder : ISeeder
{
    public static IReadOnlyList<RewardPool> DataAll =>
    [
        RewardPool.Create(
            type: RewardPoolType.Roulette,
            code: RewardPoolCode.MainRoulette,
            title: "Main Roulette",
            description: "Default roulette reward pool.",
            triggerEvents: [],
            config: new RewardPoolConfigData
            {
                Theme = "casino_gold",
                AnimationType = RewardPoolType.Roulette,
                SpinDurationMs = 3500,
                ShowWinningEffect = true,

                RequiredItemType = CatalogItemCategory.Ticket,
                RequiredItemAmount = 1,
                DailySpinLimitPerUser = 10,
                AllowDuplicateReward = true,

                AutoClaimReward = true,
                RewardExpireMinutes = 1440,
                AllowRetryRewardGrant = true,

                ShowProbability = false,
                ShowRewardPreview = true,
                EnableJackpotEffect = true,

                SpinCooldownSeconds = 10,
                EnableFraudDetection = true,

                Metadata = new()
                {
                    ["uiTheme"] = "main_roulette"
                }
            }),
        RewardPool.Create(
            type: RewardPoolType.Scratch,
            code: RewardPoolCode.NewUserScratch,
            title: "New User Scratch",
            description: "Scratch reward for first-time users.",
            triggerEvents: [],
            config: new RewardPoolConfigData
            {
                Theme = "new_user",
                AnimationType = RewardPoolType.Scratch,

                AutoClaimReward = true,
                RewardExpireMinutes = 1440,

                ShowRewardPreview = true,
                ShowWinningEffect = true,

                Metadata = new()
                {
                    ["campaign"] = "new_user"
                }
            }),
        RewardPool.Create(
            type: RewardPoolType.Gacha,
            code: RewardPoolCode.VipGacha,
            title: "VIP Gacha",
            description: "VIP premium random draw.",
            triggerEvents: [],
            config: new RewardPoolConfigData
            {
                Theme = "vip_gold",
                AnimationType = RewardPoolType.Gacha,

                RequiredItemType = CatalogItemCategory.Ticket,
                RequiredItemAmount = 5,

                AutoClaimReward = true,
                RewardExpireMinutes = 1440,

                ShowProbability = false,
                EnableJackpotEffect = true,

                Metadata = new()
                {
                    ["tier"] = "vip"
                }
            })
    ];
    
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        var existingCodes = await context.RewardPools
            .AsNoTracking()
            .Select(x => x.Code)
            .ToHashSetAsync(ct);

        var pools = DataAll
            .Where(x => !existingCodes.Contains(x.Code))
            .ToList();

        if (pools.Count == 0)
            return;
        
        await context.RewardPools.AddRangeAsync(pools, ct);
    }
}