namespace game_x.share.ExternalApi.PaymentGateway.Dtos;

public sealed record DepositOrderPayload
{
    public required int PlatformId { get; set; }
    public required string Signature { get; init; }
    public required string MerchantId { get; set; }
    public required string OrderNumber { get; set; }
    public required decimal Amount { get; set; }
    public required int ProviderId { get; set; }
    public string? UserId { get; set; }
    public string? Remark { get; set; }
}