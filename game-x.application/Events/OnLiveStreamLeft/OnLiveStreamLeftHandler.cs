using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Events.OnLiveStreamLeft;

public sealed class OnLiveStreamLeftHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IUnitOfWork unitOfWork,
    ILiveStreamChatRepo liveStreamChatRepo,
    ILiveStreamHubService liveStreamHub,
    ILiveStreamRepo liveStreamRepo) : IApplicationEventHandler<OnLiveStreamLeftEvent>
{
    public async Task Handle(OnLiveStreamLeftEvent @event, CancellationToken ct = default)
    {
        var streamKey = @event.StreamKey;
        var viewer = @event.Viewer;

        // Mark as stream view change to update viewer count
        liveStreamManager.MarkAsStreamViewChange(streamKey);

        await CreateStreamMessage(streamKey, viewer, ct);
        await liveStreamHub.NotifyUserLeft(streamKey, viewer.ViewerId);
    }

    private async Task CreateStreamMessage(string streamKey, LiveStreamViewerDto viewer, CancellationToken ct)
    {
        var schedule = await liveStreamRepo.GetByStreamKeyAsync(streamKey, ct);
        var chatMessage = LiveStreamChatMessage.Create(
            Guid.NewGuid(),
            schedule.Id,
            viewer.ViewerId,
            string.Empty,
            LiveStreamChatMessageType.UserLeft);
        await liveStreamChatRepo.CreateAsync(chatMessage, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Add message to cache in stream
        var newMessage = await liveStreamChatRepo.GetByIdAsync(chatMessage.PublicId, ct);
        var chatMessageDto = newMessage.Adapt<LiveStreamChatMessageDto>();
        liveStreamManager.AddMessageToStream(streamKey, chatMessageDto);
        await liveStreamHub.SendChatMessage(streamKey, chatMessageDto);
    }
}
