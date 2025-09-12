namespace game_x.application.Features.LiveStreams.Dtos;

public sealed class LiveStreamStatusDto
{
    public string StreamKey { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsLive { get; set; }
    public DateTime OfflineAt { get; set; }
    public string TalentId { get; set; } = string.Empty;
    public string TalentName { get; set; } = string.Empty;
    public List<string> BlackList { get; set; } = [];
}

public sealed class BlackListItemDto
{
    public string StreamKey { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public BlackListAction Action { get; set; }
    public DateTime BlockTo { get; set; }
}

public enum BlackListAction
{
    View,
    Chat,
    Donate,
}
