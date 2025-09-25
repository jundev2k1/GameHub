namespace game_x.application.Features.LiveStreams.Schedules.Dtos;

public sealed class LiveStreamScheduleCategoryDto
{
    public required Guid Id { get; set; }
    public required bool IsPrimary { get; set; }
    public required int Priority { get; set; }
}
