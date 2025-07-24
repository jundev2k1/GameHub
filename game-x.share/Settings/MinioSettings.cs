namespace game_x.share.Settings;

public sealed class MinioSettings : BaseSettings
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PublicEndpoint { get; set; } = string.Empty;
    public string InternalEndpoint { get; set; } = string.Empty;
    public bool UseSslPublic { get; set; } = true;
    public bool UseSslInternal { get; set; } = false;
}
