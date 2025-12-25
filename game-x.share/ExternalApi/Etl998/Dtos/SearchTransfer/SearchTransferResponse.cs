using System.Text.Json.Serialization;
using game_x.share.ExternalApi.Etl998.Enums;

namespace game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;

public sealed class SearchTransferResponse
{
    [JsonPropertyName("dealtype")]
    public TransferDealType DealType { get; set; }
    [JsonPropertyName("customerorderid")]
    public string CustomerOrderId { get; set; } = string.Empty;
}