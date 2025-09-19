using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.PlayStream;

public sealed class PlayStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<PlayStreamCommand>
{
    public async Task<Unit> Handle(PlayStreamCommand request, CancellationToken ct = default)
    {
        // Validate the viewer information
        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.Token)
            ?? throw new BadRequestException("Viewer information was not found.");
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        // Get the live stream status from cache
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey)
            ?? throw new NotFoundException("Live stream is not found.");

        // Check if the user is blocked from viewing the stream
        var targetBlackListItem = streamInfo.BlackList
            .FirstOrDefault(i => i.UserId == viewer.ViewerId
                && i.Action == BlackListAction.View
                && i.BlockTo > DateTime.UtcNow);
        if (targetBlackListItem != null)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "You are blocked from viewing this live stream.",
                new { Time = targetBlackListItem.BlockTo });

        liveStreamManager.WatchLiveStream(viewer);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
