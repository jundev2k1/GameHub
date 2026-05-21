using game_x.domain.Enum.Rewards;

namespace game_x.domain.ValueObjects.Missions;

public sealed class MissionConfigData
{
    #region Progress Rules

    /// <summary>
    /// Required progress count to complete.
    ///
    /// Examples:
    /// Login 7 times
    /// Deposit 3 times
    /// Share 1 time
    /// </summary>
    public int RequiredProgress { get; init; } = 1;

    /// <summary>
    /// Progress counting mode.
    /// Count = each valid event increments by 1
    /// SumValue = accumulate event values
    /// </summary>
    public MissionProgressMode ProgressMode { get; init; } = MissionProgressMode.Count;

    /// <summary>
    /// Whether progress must be consecutive.
    /// Example: daily streak login.
    /// </summary>
    public bool RequireConsecutiveProgress { get; init; }

    /// <summary>Reset streak if user misses the required interval.</summary>
    public bool ResetProgressOnMiss { get; init; }

    #endregion

    #region Value Rules

    /// <summary>
    /// Minimum event value.
    /// Example:
    /// Deposit >= 100
    /// Spend >= 50
    /// </summary>
    public decimal? MinimumValue { get; init; }

    /// <summary>Optional maximum event value.</summary>
    public decimal? MaximumValue { get; init; }

    #endregion

    #region Time Rules

    /// <summary>
    /// Minimum seconds between valid progress events.
    /// Anti abuse.
    /// </summary>
    public int ProgressCooldownSeconds { get; init; }

    /// <summary>
    /// Required interval between streak events.
    ///
    /// Example:
    /// Daily login = 1-day
    /// Weekly checkin = 7 days
    /// </summary>
    public int RequiredIntervalDays { get; init; } = 1;

    #endregion

    #region Share / Referral Rules

    /// <summary>
    /// Share conversion requirement.
    /// Example: successful clicks.
    /// </summary>
    public int RequiredShareConversions { get; init; }

    /// <summary>Count only unique users.</summary>
    public bool RequireUniqueUsersOnly { get; init; } = true;

    /// <summary>Ignore self-generated actions.</summary>
    public bool ExcludeSelfActions { get; init; } = true;

    #endregion

    #region Reward Flow

    /// <summary>
    /// Auto grant rewards immediately.
    /// false = user manually claims.
    /// </summary>
    public bool AutoClaimReward { get; init; }

    /// <summary>
    /// Reward expiration after availability.
    /// 0 = never expire.
    /// </summary>
    public int RewardExpireMinutes { get; init; }

    #endregion

    #region Limits

    /// <summary>
    /// Maximum times user can complete this mission.
    /// 0 = unlimited.
    /// </summary>
    public int MaxCompletionPerUser { get; init; }

    #endregion

    #region Extensibility

    public Dictionary<string, string>? Metadata { get; init; }

    #endregion

    public static MissionConfigData Default() => new();
}