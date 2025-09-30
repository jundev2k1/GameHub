using System.Text.Json.Serialization;

namespace game_x.application.Features.Interactions.Characters.Dtos;

public sealed class InteractionCharacterDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    [JsonIgnore]
    public int DefaultPoseId { get; set; }
    [JsonIgnore]
    public MediaFile DefaultPose { get; set; } = default!;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public InteractionCharacterPoseItemDto[] PoseSettings { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
