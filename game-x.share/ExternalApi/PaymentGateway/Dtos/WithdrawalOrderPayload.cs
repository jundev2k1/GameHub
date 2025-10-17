namespace game_x.share.ExternalApi.PaymentGateway.Dtos;

public sealed record WithdrawalOrderPayload
{
    public required int PlatformId { get; set; }
    public required string Signature { get; init; }
    public required string MerchantId { get; set; }
    public required string OrderNumber { get; set; }
    public required decimal Amount { get; set; }
    public required int ProviderId { get; set; }
    public required string WalletAddress { get; set; }
    public string? Remark { get; set; }
}