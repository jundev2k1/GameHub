using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public class GameTagDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    [JsonIgnore]
    public Dictionary<string, GameTagTranslationInfo> TagTranslations { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
