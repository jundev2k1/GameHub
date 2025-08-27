using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public sealed class GamePlatformDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    [JsonIgnore]
    public int Priority { get; set; }
    [JsonIgnore]
    public bool IsActive { get; set; } = true;
}
