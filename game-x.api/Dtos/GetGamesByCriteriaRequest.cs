using game_x.api.Common;

namespace game_x.api.Dtos;

public sealed class GetGamesByCriteriaRequest : SearchCriteriaRequest
{
    [FromQuery(Name = "types")]
    public string? Types { get; set; }
    [FromQuery(Name = "categories")]
    public string? Categories { get; set; }
    [FromQuery(Name = "tags")]
    public string? Tags { get; set; }
}
