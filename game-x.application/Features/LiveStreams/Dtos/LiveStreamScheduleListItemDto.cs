using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.LiveStreams.Dtos;

public sealed class LiveStreamScheduleListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public LiveStreamStatus Status { get; set; }
    public UserSummaryInfo? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
