namespace game_x.api.Dtos;

public sealed class UpdatePoseRequest
{
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;
    public IFormFile? Pose { get; set; }
}
