using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetGameTransactionsRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "statuses")]
    public string Statuses { get; set; } = string.Empty;
    [FromQuery(Name = "platforms")]
    public string Platforms { get; set; } = string.Empty;
}
