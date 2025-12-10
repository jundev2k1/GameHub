namespace game_x.share.Settings;

public sealed class BaccaratHmacSettings : BaseSettings
{
    public string Host { get; set; } = string.Empty;
    /// <summary>Partner/client identifier</summary>
    public string ClientId { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    /// <summary>Base64 or plain secret key</summary>
    public string Secret { get; set; } = string.Empty;
    // Allowed timestamp drift in seconds
    public int AllowedTimestampSkewSeconds { get; set; } = 300; // 5 minutes

    // Header names
    public string ClientIdHeader { get; set; } = "X-Client-Id";
    public string AppIdHeader { get; set; } = "X-AppId";
    public string SignatureHeader { get; set; } = "X-Signature";
    public string TimestampHeader { get; set; } = "X-Timestamp";
    public string NonceHeader { get; set; } = "X-Nonce";
}
