namespace game_x.share.ExternalApi.GameProvider.Dtos;

public class ResponseBase
{
    /// <summary>回傳狀態</summary>
    public bool IsSuccess { get; set; }
    /// <summary>錯誤代碼</summary>
    public string ErrorCode { get; set; } = string.Empty;
    /// <summary>錯誤說明</summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
