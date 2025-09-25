namespace game_x.api.Dtos;

public sealed class UpdateGameRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Note { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public IFormFile? Thumbnail { get; set; }
    public GameItemSettings[]? Categories { get; set; }
    public GameItemSettings[]? Types { get; set; }
    public GameItemSettings[]? Tags { get; set; }
}

public sealed class GameItemSettings
{
    public Guid Id { get; set; }
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
}
