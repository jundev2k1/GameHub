using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Dtos;

public class GameTypeDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
    [JsonIgnore]
    public Dictionary<string, GameTypeTranslationInfo> TypeTranslations { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
