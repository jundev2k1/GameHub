using System.Text.Json.Serialization;

namespace game_x.application.Features.NavigationItems.Dtos;

public class NavigationItemDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public NavigationTargetType TargetType { get; set; }
    [JsonIgnore]
    public int? TargetLocalId { get; set; }
    public Guid? TargetId { get; set; }
    public string CustomUrl { get; set; } = string.Empty;
    [JsonIgnore]
    public MediaFile? Icon { get; set; }
    public string? IconUrl { get; set; }
    public int Priority { get; set; } = 0;
    public bool IsActive { get; set; }
    [JsonIgnore]
    public Dictionary<string, NavigationItemTranslationInfo> NavigationTranslations { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
