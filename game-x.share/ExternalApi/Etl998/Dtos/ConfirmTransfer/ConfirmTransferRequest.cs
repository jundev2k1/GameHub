using System.Text.Json.Serialization;
using game_x.share.ExternalApi.Etl998.Constants;

namespace game_x.share.ExternalApi.Etl998.Dtos.ConfirmTransfer;

public sealed class ConfirmTransferRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public required string Password { get; set; } = string.Empty;
    /// <summary>Transfer amount. Must be greater than 0.</summary>
    [JsonPropertyName("credit")]
    public required decimal Credit { get; set; }

    /// <summary>Transfer a direction: "IN" = transfer in, "OUT" = transfer out.</summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; } = CreditTypes.Deposit;
    /// <summary>Unique transfer order ID (up to 64 characters).</summary>
    [JsonPropertyName("customerorderid")]
    public required string CustomerOrderId { get; set; } = string.Empty;
}