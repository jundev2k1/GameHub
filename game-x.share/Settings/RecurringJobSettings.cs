namespace game_x.share.Settings;

public sealed class RecurringJobSettings : BaseSettings
{
    public string SyncRefreshTokenJob { get; set; } = string.Empty;
    public string ExpiredTokenCleanupJob { get; set; } = string.Empty;
}
