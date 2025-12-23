using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.AccountBalance;

public sealed class AccountBalanceRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public required string Password { get; set; } = string.Empty;
}