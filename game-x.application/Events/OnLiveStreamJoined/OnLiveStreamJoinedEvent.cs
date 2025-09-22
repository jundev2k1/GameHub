using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Events.OnLiveStreamJoined;

public record OnLiveStreamJoinedEvent(string StreamKey, LiveStreamViewerDto Viewer) : IApplicationEvent;
