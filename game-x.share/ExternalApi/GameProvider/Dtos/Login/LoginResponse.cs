namespace game_x.share.ExternalApi.GameProvider.Dtos.Login;

public sealed class LoginResponse : ResponseBase
{
    /// <summary>登入成功後跳轉的頁面</summary>
    public string Url { get; set; } = string.Empty;
}
