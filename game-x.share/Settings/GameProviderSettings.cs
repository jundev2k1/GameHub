namespace game_x.share.Settings;

public sealed class GameProviderSettings : BaseSettings
{
    public required string Host { get; set; } = string.Empty;

    public required string ApiToken { get; set; } = string.Empty;

    public required string AesKey { get; set; } = string.Empty;

    public required string Iv { get; set; } = string.Empty;

    public required string RevertProxyUrl { get; set; } = string.Empty;
}
