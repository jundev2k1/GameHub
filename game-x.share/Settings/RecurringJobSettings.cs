namespace game_x.share.Settings;

public sealed class RecurringJobSettings : BaseSettings
{
    public string DashboardStatisticJob { get; set; } = string.Empty;
    public string SessionCleanUpJob { get; set; } = string.Empty;
}
