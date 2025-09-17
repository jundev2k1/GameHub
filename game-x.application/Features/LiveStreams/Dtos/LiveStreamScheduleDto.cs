using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.LiveStreams.Dtos;

public class LiveStreamScheduleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string StreamKey { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public LiveStreamStatus Status { get; set; }
    public string? CancellationReason { get; set; }
    public UserSummaryInfo? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
