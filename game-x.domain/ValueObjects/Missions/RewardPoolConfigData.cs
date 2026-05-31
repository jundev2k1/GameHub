using game_x.domain.Enum.Rewards;

namespace game_x.domain.ValueObjects.Missions;

public sealed class RewardPoolConfigData
{
    #region Basic
    public string Theme { get; init; } = "default";

    public RewardPoolType AnimationType { get; init; } = RewardPoolType.Roulette;

    public int SpinDurationMs { get; init; } = 3000;

    public bool ShowWinningEffect { get; init; } = true;
    #endregion
    
    #region Spin Rules
    /// <summary>
    /// Inventory item required to spin.
    /// Example: ticket, free_spin
    /// </summary>
    public CatalogItemCategory RequiredItemType { get; init; } = CatalogItemCategory.Ticket;

    public Guid? RequiredCatalogItemId { get; init; }
    
    /// <summary>How many items required per spin.</summary>
    public int RequiredItemAmount { get; init; } = 1;
    
    /// <summary>
    /// Maximum spins allowed per user per day.
    /// 0 = unlimited.
    /// </summary>
    public int DailySpinLimitPerUser { get; init; }

    /// <summary>Prevent duplicate reward items in the same session/event.</summary>
    public bool AllowDuplicateReward { get; init; } = true;
    #endregion

    #region Reward Flow
    /// <summary>
    /// Automatically grant reward after spin.
    /// false = user must manually claim.
    /// </summary>
    public bool AutoClaimReward { get; init; } = true;

    /// <summary>
    /// The Reward expires after X minutes.
    /// 0 = never expire.
    /// </summary>
    public int RewardExpireMinutes { get; init; }

    /// <summary>Whether failed reward grants can retry.</summary>
    public bool AllowRetryRewardGrant { get; init; } = true;
    #endregion
    
    #region UI / Client
    /// <summary>Show probabilities in the UI.</summary>
    public bool ShowProbability { get; init; }

    /// <summary>Display reward preview before spinning.</summary>
    public bool ShowRewardPreview { get; init; } = true;

    /// <summary>Enable jackpot animation/effects.</summary>
    public bool EnableJackpotEffect { get; init; } = true;
    #endregion
    
    #region Anti Abuse
    /// <summary>Cooldown between spins in seconds.</summary>
    public int SpinCooldownSeconds { get; init; }

    /// <summary>Enable anti fraud checks.</summary>
    public bool EnableFraudDetection { get; init; } = true;
    #endregion
    
    /// <summary>Flexible future config.</summary>
    public Dictionary<string, string>? Metadata { get; init; }
    
    public static RewardPoolConfigData Default()
    {
        return new();
    }
}