namespace game_x.share.ExternalApi.SasSlot.Dtos.Deposit;

public sealed class SasSlotDepositRequest
{
    public string PlatformCode { get; set; } = string.Empty;
    public string ExtUserId { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPromo { get; set; }
    public int Ts { get; set; }
    public string Nonce { get; set; } = string.Empty;
}
