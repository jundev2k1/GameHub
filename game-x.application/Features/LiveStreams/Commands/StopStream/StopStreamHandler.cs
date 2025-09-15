using game_x.application.Contract.Infrastructure.Caching;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.LiveStreams.Commands.StopStream;

public sealed class StopStreamHandler(
    IOptions<SrsSettings> settings,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<StopStreamCommand>
{
    public async Task<Unit> Handle(StopStreamCommand request, CancellationToken ct = default)
    {
        if (!settings.Value.StreamServer.EndsWith(request.Server))
            throw new BadRequestException("Server Url invalid.");

        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.Token)
            ?? throw new BadRequestException("Viewer information was not found.");
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        liveStreamManager.UnwatchLiveStream(viewer);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
