namespace game_x.application.Features.LiveStreams.Categories.Dtos;

public sealed class LiveStreamCategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsPrimary { get; set; }
}
