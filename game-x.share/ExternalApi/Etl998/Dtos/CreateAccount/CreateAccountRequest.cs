using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.CreateAccount;

public sealed class CreateAccountRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public string Password { get; set; } = String.Empty;
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;
    /// <summary>Rebate / rolling rate. Must be >= 0 and less than the parent’s rebate rate.</summary>
    [JsonPropertyName("ximalv")]
    public decimal Ximalv { get; set; }
    /// <summary>Rebate type: 1 = single side, 2 = double side.</summary>
    [JsonPropertyName("ximatype")]
    public int Ximatype { get; set; }
    /// <summary>Parent account ID.</summary>
    [JsonPropertyName("fatherId")]
    public int FatherId { get; set; }
    /// <summary>Betting limit group IDs, separated by commas. Only three IDs are allowed. Default: "1,2,3".</summary>
    [JsonPropertyName("tables")]
    public string Tables { get; set; } =  string.Empty;
}