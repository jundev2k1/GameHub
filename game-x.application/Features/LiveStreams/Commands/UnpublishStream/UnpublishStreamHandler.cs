using game_x.application.Contract.Infrastructure.Caching;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.LiveStreams.Commands.UnpublishStream;

public sealed class UnpublishStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IOptions<SrsSettings> settings) : ICommandHandler<UnpublishStreamCommand>
{
    public async Task<Unit> Handle(UnpublishStreamCommand request, CancellationToken ct = default)
    {
        if (!settings.Value.StreamServer.EndsWith(request.Server))
            throw new BadRequestException("Server Url invalid.");

        liveStreamManager.DisconnnectLiveStream(request.StreamKey);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
