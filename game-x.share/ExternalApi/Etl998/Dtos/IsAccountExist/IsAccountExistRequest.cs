using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;

public sealed class IsAccountExistRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
}