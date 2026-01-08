namespace game_x.share.Settings;

public sealed class AtgSettings : BaseSettings
{
    public string Host { get; set; } = string.Empty;
    public string XOperator { get; set; } = string.Empty;
    public string XOperatorKey { get; set; } = string.Empty;
    public int ProviderId { get; set; }
}