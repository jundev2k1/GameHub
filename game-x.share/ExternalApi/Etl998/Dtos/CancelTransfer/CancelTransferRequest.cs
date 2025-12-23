using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;

public sealed class CancelTransferRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public required string Password { get; set; } = string.Empty;
    /// <summary>Unique transfer order ID (up to 64 characters).</summary>
    [JsonPropertyName("customerorderid")]
    public required string CustomerOrderId { get; set; } = string.Empty;
    /// <summary>Start date, e.g. "2016-7-1"</summary>
    [JsonPropertyName("dateStart")]
    public required string DateStart { get; set; } = string.Empty;
    /// <summary>End date, e.g. "2016-7-1"</summary>
    [JsonPropertyName("dateEnd")]
    public required string DateEnd { get; set; } = string.Empty;
}