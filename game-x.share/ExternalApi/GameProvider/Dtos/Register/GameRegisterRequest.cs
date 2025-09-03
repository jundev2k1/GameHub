namespace game_x.share.ExternalApi.GameProvider.Dtos.Register;

public sealed class GameRegisterRequest
{
    /// <summary>帳號 請輸入3-20個首字為英文英文字母以及數字的組合</summary>
    public string Account { get; set; } = string.Empty;
    /// <summary>密碼 請輸入5-12個首字為大寫英文不可連續重複的英文字母或數字組合</summary>
    public string Passwd { get; set; } = string.Empty;
    /// <summary>別名</summary>
    public string Alias { get; set; } = string.Empty;
    /// <summary>退水設定，合法值:0, 0.1, 0.3, 0.5, 1, 1.5, 2, 2.5, 100 EX:設置為0.1 上層退水-0.1 0為退水全退 100為賺取所有退水</summary>
    public decimal Rebateset { get; set; }
}
