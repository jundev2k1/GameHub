namespace game_x.share.ExternalApi.GameProvider.Dtos.Login;

public sealed class GameLoginRequest
{
    /// <summary>帳號 請輸入3-20個首字為英文英文字母以及數字的組合</summary>
    public string Account { get; set; } = string.Empty;
    /// <summary>密碼 請輸入5-12個首字為大寫英文不可連續重複的英文字母或數字組合</summary>
    public string Passwd { get; set; } = string.Empty;
    /// <summary>遊戲代碼(代碼請參考代碼表) 遊戲地址選擇大廳時此參數無效</summary>
    public string Gamecode { get; set; } = string.Empty;
    /// <summary>語系 未設定將使用預設語系 支援:zh-Hant(繁體體中文),zh-Hants(簡體中文)</summary>
    public string Locale { get; set; } = string.Empty;
    /// <summary>遊戲地址 遊戲大廳:lobby 彩票遊戲:lotterygame</summary>
    public string Address { get; set; } = string.Empty;
    /// <summary>退出時返回網址，需使用UrlEncode進行編碼（非必填） 需包含http:// 或 https://</summary>
    public string ReturnUrl { get; set; } = string.Empty;
}
