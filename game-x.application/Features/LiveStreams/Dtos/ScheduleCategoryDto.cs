namespace game_x.application.Features.LiveStreams.Dtos;

public sealed class ScheduleCategoryDto
{
    public required Guid Id { get; set; }
    public required bool IsPrimary { get; set; }
    public required int Priority { get; set; }
}
