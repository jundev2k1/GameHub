namespace game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

public sealed class GameProviderWalletResponse
{
    public bool issuccess { get; set; }
    public WalletData data { get; set; } = new();
}

public sealed class WalletData
{
    public string currency { get; set; } = string.Empty;
    public decimal quota { get; set; }
}