using game_x.application.Contract.Infrastructure.Caching;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.LiveStreams.Commands.PlayStream;

public sealed class PlayStreamHandler(
    IOptions<SrsSettings> settings,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<PlayStreamCommand>
{
    public async Task<Unit> Handle(PlayStreamCommand request, CancellationToken ct = default)
    {
        if (!settings.Value.StreamServer.EndsWith(request.Server))
            throw new BadRequestException("Server Url invalid.");

        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.ClientId);
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        liveStreamManager.WatchLiveStream(viewer);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
