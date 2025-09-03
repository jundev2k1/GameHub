using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetBankAccountListRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "currency")]
    public string CurrencyCode { get; set; } = string.Empty;
}
