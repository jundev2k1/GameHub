namespace game_x.api.Dtos;

public sealed class GetGamesRequest
{
    [FromQuery(Name = "search")]
    public string? Keyword { get; set; }
    [FromQuery(Name = "platform")]
    public Guid? Platform { get; set; }
    [FromQuery(Name = "category")]
    public Guid[]? Category { get; set; }
    [FromQuery(Name = "type")]
    public Guid[]? GameType { get; set; }
    [FromQuery(Name = "page")]
    public int? PageNumber { get; set; } = 1;
    [FromQuery(Name = "pageSize")]
    public int? PageSize { get; set; } = 20;
}
