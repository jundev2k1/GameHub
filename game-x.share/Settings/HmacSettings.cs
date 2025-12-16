namespace game_x.share.Settings;

public sealed class HmacSettings : BaseSettings
{
    // Allowed timestamp drift in seconds
    public int AllowedTimestampSkewSeconds { get; set; } = 300; // 5 minutes

    // Replay cache TTL in seconds
    public int NonceTtlSeconds { get; set; } = 300;

    // Header names
    public string PartnerHeader { get; set; } = "X-Partner";
    public string SignatureHeader { get; set; } = "X-Signature";
    public string TimestampHeader { get; set; } = "X-Timestamp";
    public string NonceHeader { get; set; } = "X-Nonce";
}
