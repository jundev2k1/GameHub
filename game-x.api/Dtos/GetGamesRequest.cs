namespace game_x.api.Dtos;

public sealed class GetGamesRequest
{
    [FromQuery(Name = "search")]
    public string? Keyword { get; set; }
    [FromQuery(Name = "platform")]
    public Guid? Platform { get; set; }
    [FromQuery(Name = "categories")]
    public Guid[]? Categories { get; set; }
    [FromQuery(Name = "types")]
    public Guid[]? GameTypes { get; set; }
    [FromQuery(Name = "tags")]
    public Guid[]? GameTags { get; set; }
    [FromQuery(Name = "active")]
    public bool? IsActive { get; set; }
    [FromQuery(Name = "page")]
    public int? PageNumber { get; set; } = 1;
    [FromQuery(Name = "pageSize")]
    public int? PageSize { get; set; } = 20;
}
