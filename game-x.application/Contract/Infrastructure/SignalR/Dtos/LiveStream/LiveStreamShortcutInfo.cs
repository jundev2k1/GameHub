namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;

public sealed class LiveStreamShortcutInfo
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string StreamKey { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
