using System.Text.Json.Serialization;

namespace game_x.application.Features.Interactions.Characters.Dtos;

public sealed class InteractionCharacterPoseItemDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int Priority { get; set; }
    [JsonIgnore]
    public int PoseId { get; set; }
    [JsonIgnore]
    public MediaFile Pose { get; set; } = default!;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
