namespace game_x.share.Settings;

public sealed class HookTestSettings : BaseSettings
{
    public string DummySecretKey { get; set; } = string.Empty;
    public string MemberCreatedSecretKey { get; set; } = string.Empty;
    public string IbUpgradedSecretKey { get; set; } = string.Empty;
}