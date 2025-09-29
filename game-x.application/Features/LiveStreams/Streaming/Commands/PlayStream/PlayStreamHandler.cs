using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnLiveStreamJoined;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.share.Extensions;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.PlayStream;

public sealed class PlayStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<PlayStreamCommand>
{
    public async Task<Unit> Handle(PlayStreamCommand request, CancellationToken ct = default)
    {
        // Validate the viewer information
        var viewer = liveStreamManager.GetViewerInfo(request.StreamKey, request.Token)
            ?? throw new BadRequestException("Viewer information was not found.");
        if (viewer.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");

        if (viewer.ClientId.IsNotNullOrEmpty() && viewer.ClientId != request.ClientId)
            throw new ForbiddenException("Client ID is invalid.");

        // Update client ID if not exists
        if (viewer.ClientId.IsNullOrEmpty())
            viewer.ClientId = request.ClientId;

        // Get the live stream status from cache
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey)
            ?? throw new NotFoundException("Live stream is not found.");

        // Check if the user is blocked from viewing the stream
        var targetBlackListItem = streamInfo.BlackList
            .FirstOrDefault(i => i.UserId == viewer.ViewerId
                && i.Action == BlackListAction.View
                && i.BanUntil > DateTime.UtcNow);
        if (targetBlackListItem != null)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "You are blocked from viewing this live stream.",
                new { Time = targetBlackListItem.BanUntil });

        // Mark as watching stream
        liveStreamManager.WatchLiveStream(viewer);

        // Add chat and notify for host
        var @event = new OnLiveStreamJoinedEvent(request.StreamKey, viewer);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
