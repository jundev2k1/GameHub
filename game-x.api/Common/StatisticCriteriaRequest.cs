namespace game_x.api.Common;

public sealed class StatisticCriteriaRequest
{
    [FromQuery(Name = "filter")]
    public string[]? Filters { get; set; } = [];
}