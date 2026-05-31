namespace game_x.share.Settings;

public sealed class JwtSettings : BaseSettings
{
    public double DurationInMinutes { get; set; }

    public required string Key { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required string Provider { get; set; }

    public required string Version { get; set; }
}
