namespace game_x.api.Dtos;

public sealed class UpdateGameRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Note { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public IFormFile? Thumbnail { get; set; }
    public string? Categories { get; set; }
    public string? Types { get; set; }
    public string? Tags { get; set; }
}
