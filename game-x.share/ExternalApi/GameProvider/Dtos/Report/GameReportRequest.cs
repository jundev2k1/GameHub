namespace game_x.share.ExternalApi.GameProvider.Dtos.Report;

public sealed class GameReportRequest
{
    /// <summary>開始時間</summary>
    public string StartDate { get; set; } = string.Empty;
    /// <summary>結束時間</summary>
    public string EndDate { get; set; } = string.Empty;
}
