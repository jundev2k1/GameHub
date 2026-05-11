using game_x.domain.Enum.Missions;

namespace game_x.domain.ValueObjects.Missions;

public sealed class MissionConfigData
{
    #region Progress Rules
    /// <summary>
    /// Progress required to complete a mission.
    /// Example: login 7 days, share 1 time, deposit 3 times
    /// </summary>
    public int RequiredProgress { get; init; } = 1;

    /// <summary>
    /// Whether progress must be consecutive.
    /// Example: daily login streak.
    /// </summary>
    public bool RequireConsecutiveProgress { get; init; }

    /// <summary>Reset progress if user misses streak.</summary>
    public bool ResetProgressOnMiss { get; init; }
    #endregion

    #region Value Rules
    /// <summary>
    /// Minimum required amount/value.
    /// Example: deposit >= 100, spend >= 50
    /// </summary>
    public decimal? MinimumValue { get; init; }

    /// <summary>Maximum allowed value if needed.</summary>
    public decimal? MaximumValue { get; init; }
    #endregion
    
    #region Event Rules
    /// <summary>
    /// Allowed event types for mission completion.
    /// Example: Deposit, ShareClick, Login
    /// </summary>
    public List<MissionType> AllowedEventTypes { get; init; } = [];
    #endregion
    
    #region Share / Referral Rules
    /// <summary>Required successful share clicks.</summary>
    public int RequiredShareClicks { get; init; }

    /// <summary>Count only unique users.</summary>
    public bool RequireUniqueUsersOnly { get; init; } = true;

    /// <summary>Ignore self-clicks.</summary>
    public bool ExcludeSelfActions { get; init; } = true;
    #endregion

    #region Reward Flow
    /// <summary>Auto claim reward after completion.</summary>
    public bool AutoClaimReward { get; init; }

    /// <summary> Reward claim expiration in minutes. 0 = never expire.</summary>
    public int RewardExpireMinutes { get; init; }
    #endregion

    #region Limits
    /// <summary> Maximum completions per user. 0 = unlimited.</summary>
    public int MaxCompletionPerUser { get; init; }

    /// <summary>Cooldown between valid progress events. Anti abuse.</summary>
    public int ProgressCooldownSeconds { get; init; }
    #endregion

    public Dictionary<string, string>? Metadata { get; init; }

    public static MissionConfigData Default()
    {
        return new();
    }
}