namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public sealed class LiveStreamViewerDto
{
    public string StreamKey { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ViewerId { get; set; } = string.Empty;
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerAvatar { get; set; }
    public string DeviceInfo { get; set; } = string.Empty;
    public bool IsWatching { get; set; }
    public DateTime? JoinAt { get; set; }
    public DateTime? OutAt { get; set; }
}
