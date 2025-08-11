namespace game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

public sealed class WalletRequest
{
    /// <summary>帳號 請輸入3-20個首字為英文英文字母以及數字的組合</summary>
    public string Account { get; set; } = string.Empty;
}
