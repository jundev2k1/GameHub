using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetBankAccountListRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "currencies")]
    public string CurrencyCode { get; set; } = string.Empty;
    [FromQuery(Name = "statuses")]
    public string Statuses { get; set; } = string.Empty;
}
