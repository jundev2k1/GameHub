using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.PaymentGateway.Dtos;

public sealed record PaymentGatewayCallbackRequest
{
    [JsonPropertyOrder(0)] public required string TransactionId { get; set; }
    [JsonPropertyOrder(1)] public required string MerchantOrderId { get; set; }
    [JsonPropertyOrder(2)] public required string MerchantId { get; set; }
    [JsonPropertyOrder(3)] public required string TransactionType { get; set; }
    [JsonPropertyOrder(4)] public required string Status { get; set; }
    [JsonPropertyOrder(5)] public required decimal Amount { get; set; }
    [JsonPropertyOrder(6)] public required decimal FinalAmount { get; set; }
    [JsonPropertyOrder(7)] public required string Currency { get; set; }
    [JsonPropertyOrder(8)] public required string ProviderOrderId { get; set; }
    [JsonPropertyOrder(9)] public DateTime? CreatedAt { get; set; }
    [JsonPropertyOrder(10)] public DateTime? CompletedAt { get; set; }
    [JsonPropertyOrder(11)] public required string Remark { get; set; }
    [JsonPropertyOrder(12)] public required string TransactionHash { get; set; }
    [JsonPropertyOrder(13)] public string? DepositWalletAddress { get; set; } = null!;
    [JsonPropertyOrder(14)] public string? WithdrawalWalletAddress { get; set; } = null!;
}