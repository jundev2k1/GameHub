using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Events.OnLiveStreamLeft;

public record OnLiveStreamLeftEvent(string StreamKey, LiveStreamViewerDto Viewer) : IApplicationEvent;
