using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Features.LiveStreams.Commands.StopStream;

public sealed class StopStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<StopStreamCommand>
{
    public async Task<Unit> Handle(StopStreamCommand request, CancellationToken ct = default)
    {
        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.Token)
            ?? throw new BadRequestException("Viewer information was not found.");
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        liveStreamManager.UnwatchLiveStream(viewer);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
