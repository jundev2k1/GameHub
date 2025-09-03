namespace game_x.share.ExternalApi.GameProvider.Dtos.Report;

public sealed class GameReportResponse : ResponseBase
{
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public ReportItem[] Data { get; set; } = [];
}

public sealed class ReportItem
{
    public string WagersId { get; set; } = string.Empty;
    public string GameCode { get; set; } = string.Empty;
    public DateTime NewTime { get; set; }
    public DateTime BalanceTime { get; set; }
    public string Account { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string GameNum { get; set; } = string.Empty;
    public string[] Content { get; set; } = [];
    public decimal Gold { get; set; }
    public decimal RealGold { get; set; }
    public string Rebate { get; set; } = string.Empty;
    public decimal WinGold { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
}