using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.PaymentGateway.Dtos;

public sealed class OrderResponse
{
    [JsonPropertyOrder(0)] public bool Success { get; set; }
    [JsonPropertyOrder(1)] public string TransactionId { get; set; } = string.Empty;
    [JsonPropertyOrder(2)] public string OrderNumber { get; set; } = string.Empty;
    [JsonPropertyOrder(3)] public decimal Amount { get; set; }
    [JsonPropertyOrder(4)] public string Status { get; set; } = string.Empty;
    [JsonPropertyOrder(5)] public string WalletAddress{ get; set; } = string.Empty;
    [JsonPropertyOrder(6)] public string Message { get; set; } = string.Empty;
}