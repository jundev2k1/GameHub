using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Categories.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Schedules.Dtos;

public sealed class LiveStreamScheduleClientItemDto
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public string StreamKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public LiveStreamCategorySummaryDto[] Categories { get; set; } = [];
    public LiveStreamStatus Status { get; set; }
    public UserSummaryInfo? AssignedTo { get; set; }
    public int ViewCount { get; set; }
    public NotificationChannel[] ReminderChannels { get; set; } = [];
}
