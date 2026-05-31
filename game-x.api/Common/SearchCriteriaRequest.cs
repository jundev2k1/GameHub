namespace game_x.api.Common;

public class SearchCriteriaRequest
{
    [FromQuery(Name = "search")]
    public string? Keyword { get; set; }
    [FromQuery(Name = "filter")]
    public string[]? Filters { get; set; } = [];
    [FromQuery(Name = "sort")]
    public string[]? Sorts { get; set; } = [];
    [FromQuery(Name = "page")]
    public int? PageNumber { get; set; } = 1;
    [FromQuery(Name = "pageSize")]
    public int? PageSize { get; set; } = 20;
}
