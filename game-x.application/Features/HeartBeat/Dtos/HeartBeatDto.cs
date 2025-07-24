namespace game_x.application.Features.HeartBeat.Dtos;

public sealed class HeartBeatDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public DateTime LastSeenTime { get; set; } = DateTime.UtcNow;
    public bool IsOnline { get; set; }
}
