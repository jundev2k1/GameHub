using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;
using game_x.persistence.Seeds.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class MissionSeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        var missions = new[]
        {
            Mission.Create(
                code: MissionCode.DailyStreakLogin,
                type: MissionType.DailyLogin,
                title: "Daily Login Streak",
                description: "Login 7 consecutive days to unlock milestone rewards.",
                resetType: MissionResetType.Weekly,
                configData: new MissionConfigData
                {
                    RequiredProgress = 7,
                    ProgressMode = MissionProgressMode.Count,
                    RequireConsecutiveProgress = true,
                    ResetProgressOnMiss = true,
                    AllowedUserEvents = [UserEventType.DailyLogin],
                    RequiredIntervalDays = 1,
                    ProgressCooldownSeconds = 60,
                    AutoClaimReward = false,
                    RewardExpireMinutes = 1440,
                    MaxCompletionPerUser = 0,
                    Metadata = new Dictionary<string, string> { ["uiTheme"] = "weekly_checkin" }
                }
            ),

            Mission.Create(
                code: MissionCode.FirstDeposit,
                type: MissionType.Deposit,
                title: "First Deposit Bonus",
                description: "Make your first deposit of at least 100 USD.",
                resetType: MissionResetType.Never,
                configData: new MissionConfigData
                {
                    RequiredProgress = 1,
                    ProgressMode = MissionProgressMode.Count,
                    AllowedUserEvents = [UserEventType.DepositCompleted],
                    MinimumValue = 100m,
                    ProgressCooldownSeconds = 10,
                    AutoClaimReward = true,
                    RewardExpireMinutes = 0,
                    MaxCompletionPerUser = 1,
                    Metadata = new Dictionary<string, string> { ["currency"] = "USD" }
                }
            ),

            Mission.Create(
                code: MissionCode.DepositAccumulation,
                type: MissionType.Deposit,
                title: "Deposit Accumulation",
                description: "Accumulate total deposits of 1000 USD.",
                resetType: MissionResetType.Never,
                configData: new MissionConfigData
                {
                    RequiredProgress = 1000,
                    ProgressMode = MissionProgressMode.SumValue,
                    AllowedUserEvents = [UserEventType.DepositCompleted],
                    MinimumValue = 1m,
                    ProgressCooldownSeconds = 10,
                    AutoClaimReward = true,
                    RewardExpireMinutes = 0,
                    MaxCompletionPerUser = 1,
                    Metadata = new Dictionary<string, string> { ["currency"] = "USD" }
                }
            ),

            Mission.Create(
                code: MissionCode.SocialShare,
                type: MissionType.Share,
                title: "Social Share",
                description: "Share once successfully.",
                resetType: MissionResetType.Daily,
                configData: new MissionConfigData
                {
                    RequiredProgress = 1,
                    ProgressMode = MissionProgressMode.Count,
                    AllowedUserEvents = [UserEventType.ShareCompleted],
                    AutoClaimReward = true,
                    RewardExpireMinutes = 0,
                    MaxCompletionPerUser = 0
                }
            ),

            Mission.Create(
                code: MissionCode.ShareConversion,
                type: MissionType.Share,
                title: "Invite Friends",
                description: "Get 3 unique successful referral clicks.",
                resetType: MissionResetType.Daily,
                configData: new MissionConfigData
                {
                    RequiredProgress = 3,
                    ProgressMode = MissionProgressMode.Count,
                    AllowedUserEvents = [UserEventType.ShareConverted],
                    RequireUniqueUsersOnly = true,
                    ExcludeSelfActions = true,
                    ProgressCooldownSeconds = 5,
                    AutoClaimReward = false,
                    RewardExpireMinutes = 1440,
                    MaxCompletionPerUser = 0
                }
            )
        };

        foreach (var mission in missions)
        {
            var exists = await context.Missions
                .AsNoTracking()
                .AnyAsync(x => x.Code == mission.Code, ct);

            if (exists) continue;

            await context.Missions.AddAsync(mission, ct);
        }
        
        await context.SaveChangesAsync(ct);
    }
}