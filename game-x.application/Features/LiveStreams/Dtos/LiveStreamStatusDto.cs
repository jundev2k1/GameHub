using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Enum;

namespace game_x.application.Features.LiveStreams.Dtos;

public sealed class LiveStreamStatusDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string StreamKey { get; set; } = string.Empty;
    public bool IsLive { get; set; }
    public DateTime? OfflineAt { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public LiveStreamCategorySummaryDto[] Categories { get; set; } = [];
    public UserSummaryInfo? AssignedTo { get; set; }
    public string TalentId { get; set; } = string.Empty;
    public string TalentName { get; set; } = string.Empty;
    public List<BlackListItemDto> BlackList { get; set; } = [];
}

public sealed class BlackListItemDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public BlackListAction Action { get; set; }
    public DateTime BlockTo { get; set; }
    public BlockReasonEnum Reason { get; set; }
}

public enum BlackListAction
{
    View,
    Chat,
    Donate,
}
