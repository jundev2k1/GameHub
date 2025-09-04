using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetTransactionsRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "statuses")]
    public string TransactionStatuses { get; set; } = string.Empty;
}
