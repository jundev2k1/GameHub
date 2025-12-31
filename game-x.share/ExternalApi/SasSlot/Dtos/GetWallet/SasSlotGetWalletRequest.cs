namespace game_x.share.ExternalApi.SasSlot.Dtos.GetWallet;

public sealed class SasSlotGetWalletRequest
{
    public string PlatformCode { get; set; } = string.Empty;
    public string ExtUserId { get; set; } = string.Empty;
    public int Ts { get; set; }
    public string Nonce { get; set; } = string.Empty;
}
