using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Events.LiveStream.OnLiveStreamLeft;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.StopStream;

public sealed class StopStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<StopStreamCommand>
{
    public async Task<Unit> Handle(StopStreamCommand request, CancellationToken ct = default)
    {
        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.Token)
            ?? throw new BadRequestException("Viewer information was not found.");
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        liveStreamManager.UnwatchLiveStream(viewer);

        // Add chat and notify for host
        var @event = new OnLiveStreamLeftEvent(request.StreamKey, viewer);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
