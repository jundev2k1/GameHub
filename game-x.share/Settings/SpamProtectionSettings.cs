namespace game_x.share.Settings;

public sealed class SpamProtectionSettings : BaseSettings
{
    /// <summary>
    /// The maximum number of failed verification attempts allowed 
    /// within the sliding time window before the user is temporarily locked.
    /// </summary>
    public required int MaxVerifyAttempts { get; set; }

    /// <summary>
    /// Lock duration (in minutes) applied after a user exceeds the maximum allowed failed attempts.
    /// </summary>
    public required int VerifyLockMinutes { get; set; }
    /// <summary>
    /// Gets the lock duration as a TimeSpan.
    /// </summary>
    public TimeSpan VerifyLockDuration => TimeSpan.FromMinutes(VerifyLockMinutes);

    /// <summary>
    /// The sliding time window (in minutes) in which failed attempts are counted.
    /// After this window expires without any new failures, the count resets.
    /// </summary>
    public required int VerifySlidingMinutes { get; set; }
    /// <summary>
    /// Gets the sliding window duration as a TimeSpan.
    /// </summary>
    public TimeSpan VerifySlidingWindow => TimeSpan.FromMinutes(VerifySlidingMinutes);

    /// <summary>
    /// Minimum interval (in seconds) between sending verification emails
    /// to the same user to prevent email spam.
    /// </summary>
    public required int VerifyEmailCooldownSeconds { get; set; }
    /// <summary>
    /// Gets the email cooldown duration as a TimeSpan.
    /// </summary>
    public TimeSpan VerifyEmailCooldown => TimeSpan.FromSeconds(VerifyEmailCooldownSeconds);
}
