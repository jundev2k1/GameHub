using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.PaymentGateway.Dtos;

public sealed record DepositOrderRequest
{
    [JsonPropertyOrder(0)] public required int ProviderId { get; set; }
    [JsonPropertyOrder(1)] public required string OrderNumber { get; set; }
    [JsonPropertyOrder(2)] public required decimal Amount { get; set; }
    [JsonPropertyOrder(3)] public string? UserId { get; set; }
    [JsonPropertyOrder(4)] public string? Remark { get; set; }
}