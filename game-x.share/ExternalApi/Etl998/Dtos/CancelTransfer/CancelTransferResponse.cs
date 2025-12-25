using System.Text.Json.Serialization;
using game_x.share.ExternalApi.Etl998.Enums;

namespace game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;

public sealed class CancelTransferResponse
{
    [JsonPropertyName("dealtype")]
    public TransferDealType DealType { get; set; }
    [JsonPropertyName("customerorderid")]
    public string CustomerOrderId { get; set; } = string.Empty;
}