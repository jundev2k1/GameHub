using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetKycsRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "statuses")]
    public string Statuses { get; set; } = string.Empty;
}
