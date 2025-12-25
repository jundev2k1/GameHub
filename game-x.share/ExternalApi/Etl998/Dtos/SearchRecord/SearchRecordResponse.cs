using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;

public sealed class SearchRecordResponse
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;
    [JsonPropertyName("gametype")]
    public string GameType { get; set; } = string.Empty;
    [JsonPropertyName("Tableid")]
    public string TableId { get; set; } = string.Empty;
    [JsonPropertyName("Chang")]
    public string ShoeNumber { get; set; } = string.Empty;
    /// <summary>Hand / round number.</summary>
    [JsonPropertyName("Ci")]
    public string RoundNumber { get; set; } = string.Empty;
    /// <summary>Banker / Dragon result.</summary>
    [JsonPropertyName("Zhuang")]
    public string SystemResult { get; set; } = string.Empty;
    /// <summary>Player / Tiger result.</summary>
    [JsonPropertyName("Xian")]
    public string PlayerResult { get; set; } = string.Empty;
    [JsonPropertyName("He")]
    public string Tie { get; set; } = string.Empty;
    [JsonPropertyName("Zhuangdui")]
    public string BankerPair { get; set; } = string.Empty;
    [JsonPropertyName("Xiandui")]
    public string PlayerPair { get; set; } = string.Empty;
    /// <summary>Rolling / rebate amount.</summary>
    [JsonPropertyName("Ximaliang")]
    public string Amount { get; set; } = string.Empty;
    [JsonPropertyName("Jieguo")]
    public string Result { get; set; } = string.Empty;
    /// <summary>IP address.</summary>
    [JsonPropertyName("IP")]
    public string Ip { get; set; } = string.Empty;
    [JsonPropertyName("Time")]
    public string BetTime { get; set; } = string.Empty;
    [JsonPropertyName("Jiesuantime")]
    public string SettlementTime { get; set; } = string.Empty;
    /// <summary>Balance after settlement.</summary>
    [JsonPropertyName("Smoney")]
    public string BalanceAfter { get; set; } = string.Empty;
}