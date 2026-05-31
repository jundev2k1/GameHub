namespace game_x.share.ExternalApi.SasSlot.Dtos.GetWallet;

public sealed class SasSlotGetWalletResponse
{
    public bool Success { get; set; }
    public decimal TotalBalance { get; set; }
    public SasSlotGetWalletItem? Cash { get; set; }
    public SasSlotGetWalletItem? Promo { get; set; }
}

public sealed class SasSlotGetWalletItem
{
    public decimal Total { get; set; }
    public decimal Available { get; set; }
}