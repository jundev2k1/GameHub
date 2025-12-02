using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetSystemTransactionsByCriteriaRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "type")]
    public string? TxType { get; set; }
}
