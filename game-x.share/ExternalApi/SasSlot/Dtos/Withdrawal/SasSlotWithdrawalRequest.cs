namespace game_x.share.ExternalApi.SasSlot.Dtos.Withdrawal;

public sealed class SasSlotWithdrawalRequest
{
    public string PlatformCode { get; set; } = string.Empty;
    public string ExtUserId { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;
    public bool IsPromo { get; set; }
    public int Ts { get; set; }
    public string Nonce { get; set; } = string.Empty;
}
