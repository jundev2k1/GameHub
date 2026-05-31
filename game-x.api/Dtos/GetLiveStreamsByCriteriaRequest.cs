using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetLiveStreamsByCriteriaRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "statuses")]
    public string? Statuses { get; set; }
}
