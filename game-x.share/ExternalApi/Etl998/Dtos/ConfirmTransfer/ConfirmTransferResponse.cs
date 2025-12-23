using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.ConfirmTransfer;

public sealed class ConfirmTransferResponse
{
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;
    [JsonPropertyName("money")]
    public decimal Money { get; set; }
    [JsonPropertyName("lockmoney")]
    public decimal LockMoney { get; set; }
    [JsonPropertyName("customerorderid")]
    public string CustomerOrderId { get; set; } = string.Empty;
}