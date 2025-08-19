namespace game_x.api.Common;

/// <param name="limit">Page size (1..100). Default 20.</param>
/// <param name="cursor">Opaque cursor returned from previous page.</param>
/// <param name="q">a common convention for free-text search keywords; full-text.</param>
public sealed class CursorCriteriaRequest
{
    [FromQuery(Name = "cursor")]
    public string? Cursor { get; set; }
    [FromQuery(Name = "q")]
    public string? Q { get; set; }
    [FromQuery(Name = "search")]
    public string? Search { get; set; }
    [FromQuery(Name = "filter")]
    public string[]? Filters { get; set; } = [];
    [FromQuery(Name = "sort")]
    public string[]? Sorts { get; set; } = [];
    [FromQuery(Name = "limit")]
    public int? Limit { get; set; } = 20;
}