namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public sealed class LiveStreamViewCountDto
{
    public string StreamKey { get; set; } = string.Empty;
    public int Count { get; set; }
}
