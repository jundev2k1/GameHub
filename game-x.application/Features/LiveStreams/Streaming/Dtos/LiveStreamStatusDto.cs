using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Categories.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Enum;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public sealed class LiveStreamStatusDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [JsonIgnore]
    public int? ThumbnailId { get; set; }
    public string? Thumbnail { get; set; }
    public string StreamKey { get; set; } = string.Empty;
    public bool IsLive { get; set; }
    public DateTime? LiveAt { get; set; }
    public DateTime? OfflineAt { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public LiveStreamCategorySummaryDto[] Categories { get; set; } = [];
    public string ClientId { get; set; } = string.Empty;
    public UserSummaryInfo? AssignedTo { get; set; }
    public List<BlackListItemDto> BlackList { get; set; } = [];
}

public sealed class BlackListItemDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public BlackListAction Action { get; set; }
    public DateTime BanUntil { get; set; }
    public BlockReasonEnum Reason { get; set; }
}

public enum BlackListAction
{
    View,
    Chat,
    Donate,
}
