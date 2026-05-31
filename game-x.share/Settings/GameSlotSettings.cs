namespace game_x.share.Settings;

public sealed class GameSlotSettings : BaseSettings
{
    public string Host { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string LoginUrl { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;

    // Header names
    public string SignatureAlgHeader { get; set; } = "X-Signature-Alg";
    public string SignatureHeader { get; set; } = "X-Signature";
    public string KeyIdHeader { get; set; } = "X-Key-Id";
}