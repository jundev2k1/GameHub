using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.Wallet;

public sealed class Etl998WalletResponse
{
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;
    [JsonPropertyName("money")]
    public decimal Money { get; set; }
    [JsonPropertyName("lockmoney")]
    public decimal LockMoney { get; set; }
}