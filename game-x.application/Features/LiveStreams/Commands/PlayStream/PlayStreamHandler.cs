using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Features.LiveStreams.Commands.PlayStream;

public sealed class PlayStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<PlayStreamCommand>
{
    public async Task<Unit> Handle(PlayStreamCommand request, CancellationToken ct = default)
    {
        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.Token)
            ?? throw new BadRequestException("Viewer information was not found.");
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        liveStreamManager.WatchLiveStream(viewer);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
