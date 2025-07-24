namespace game_x.share.Settings;

public sealed class JwtSettings : BaseSettings
{
    // 過期時間
    public double DurationInMinutes { get; set; }

    public required string Key { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    // Asp Identity 保存在 AspNetUserTokens.LoginProvider
    public required string Provider { get; set; }

    // Asp Identity 保存在 AspNetUserTokens.Name
    public required string Version { get; set; }
}
