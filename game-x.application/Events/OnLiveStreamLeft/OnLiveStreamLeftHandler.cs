using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Events.OnLiveStreamLeft;

public sealed class OnLiveStreamLeftHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IUnitOfWork unitOfWork,
    ILiveStreamChatRepo liveStreamChatRepo,
    ILiveStreamHubService liveStreamHub) : IApplicationEventHandler<OnLiveStreamLeftEvent>
{
    public async Task Handle(OnLiveStreamLeftEvent @event, CancellationToken ct = default)
    {
        var streamKey = @event.StreamKey;
        var viewer = @event.Viewer;

        await CreateStreamMessage(streamKey, viewer, ct);
        await liveStreamHub.NotifyUserLeft(streamKey, viewer.ViewerId);
    }

    private async Task CreateStreamMessage(string streamKey, LiveStreamViewerDto viewer, CancellationToken ct)
    {
        var chatMessage = LiveStreamChatMessage.Create(
            viewer.ViewerId,
            string.Empty,
            LiveStreamChatMessageType.UserLeft);
        await liveStreamChatRepo.CreateAsync(chatMessage, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Add message to cache in stream
        var chatMessageDto = chatMessage.Adapt<LiveStreamChatMessageDto>();
        liveStreamManager.AddMessageToStream(streamKey, chatMessageDto);
        await liveStreamHub.SendChatMessage(streamKey, chatMessageDto);
    }
}
