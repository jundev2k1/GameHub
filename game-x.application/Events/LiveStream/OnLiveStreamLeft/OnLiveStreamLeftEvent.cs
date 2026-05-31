using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Events.LiveStream.OnLiveStreamLeft;

public record OnLiveStreamLeftEvent(string StreamKey, LiveStreamViewerDto Viewer) : IApplicationEvent;
