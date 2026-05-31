namespace game_x.api.Dtos;

public sealed class CreateCharacterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public IFormFile DefaultPose { get; set; } = default!;
}
