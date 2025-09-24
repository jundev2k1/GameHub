namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public sealed class LiveStreamViewerInfoDto
{
    public string ViewerId { get; set; } = string.Empty;
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerAvatar { get; set; } = string.Empty;
    public ViewerDeviceInfoDto[] DeviceInfos { get; set; } = [];
    public bool IsWatching => DeviceInfos.Any(di => di.IsWatching);
    public DateTime? JoinAt => DeviceInfos.Length > 0 ? DeviceInfos.Min(di => di.JoinAi) : null;
    public DateTime? OutAt => DeviceInfos.Length > 0 ? DeviceInfos.Max(di => di.OutAt) : null;
}

public sealed class ViewerDeviceInfoDto
{
    public string DeviceName { get; set; } = string.Empty;
    public bool IsWatching { get; set; }
    public DateTime? JoinAi { get; set; }
    public DateTime? OutAt { get; set; }
}
