namespace game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

public sealed class WalletResponse : ResponseBase
{
    /// <summary>幣別</summary>
    public string Currency { get; set; } = string.Empty;
    /// <summary>額度 小數點2位</summary>
    public decimal Quota { get; set; }
}
