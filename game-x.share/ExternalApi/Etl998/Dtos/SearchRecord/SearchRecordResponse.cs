using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;

public sealed class SearchRecordResponse
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;
    [JsonPropertyName("gametype")]
    public int GameType { get; set; }
    [JsonPropertyName("Tableid")]
    public int TableId { get; set; }
    [JsonPropertyName("Chang")]
    public int ShoeNumber { get; set; }
    /// <summary>Hand / round number.</summary>
    [JsonPropertyName("Ci")]
    public int RoundNumber { get; set; }
    /// <summary>Banker / Dragon result.</summary>
    [JsonPropertyName("Zhuang")]
    public int SystemResult { get; set; }
    /// <summary>Player / Tiger result.</summary>
    [JsonPropertyName("Xian")]
    public int PlayerResult { get; set; }
    [JsonPropertyName("He")]
    public int Tie { get; set; }
    [JsonPropertyName("Zhuangdui")]
    public int BankerPair { get; set; }
    [JsonPropertyName("Xiandui")]
    public int PlayerPair { get; set; }
    /// <summary>Rolling / rebate amount.</summary>
    [JsonPropertyName("Ximaliang")]
    public int Amount { get; set; }
    [JsonPropertyName("Jieguo")]
    public int Result { get; set; }
    /// <summary>IP address.</summary>
    [JsonPropertyName("IP")]
    public string Ip { get; set; } = string.Empty;
    [JsonPropertyName("Time")]
    public string BetTime { get; set; } = string.Empty;
    [JsonPropertyName("Jiesuantime")]
    public string SettlementTime { get; set; } = string.Empty;
    /// <summary>Balance after settlement.</summary>
    [JsonPropertyName("Smoney")]
    public int BalanceAfter { get; set; }
}