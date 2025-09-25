using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Categories.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.PublishStream;

public sealed class PublishStreamHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IFileManagerCacheService fileManagerCache,
    ILiveStreamHubService liveStreamHub) : ICommandHandler<PublishStreamCommand>
{
    public async Task<Unit> Handle(PublishStreamCommand request, CancellationToken ct = default)
    {
        var streamSetting = await liveStreamRepo
            .GetByStreamKeyAsync(request.StreamKey, ct);
        if (streamSetting.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");
        if (streamSetting.AssignedId is null)
            throw new ForbiddenException("Stream key is not assigned to any talent.");
        if (streamSetting.Status == LiveStreamStatus.Ended || streamSetting.Status == LiveStreamStatus.Cancelled)
            throw new ForbiddenException("Stream has ended or been canceled.");

        // Initialize stream info in cache if not exists
        if (!liveStreamManager.IsExistLiveStream(streamSetting.StreamKey))
        {
            if (streamSetting.EndTime < DateTime.UtcNow)
                throw new ForbiddenException("Stream has ended.");

            await InitStreamInfo(streamSetting, request.ClientId, ct);
        }

        // Check if the stream has ended for more than 10 minutes
        var streamInfo = liveStreamManager.GetLiveStreamStatus(streamSetting.StreamKey);
        if (!streamInfo!.IsLive
            && streamInfo.EndTime < DateTime.UtcNow
            && streamInfo.OfflineAt.HasValue
            && ((DateTime.UtcNow - streamInfo.OfflineAt.Value).Minutes > 10))
            throw new ForbiddenException("Stream has ended.");

        // Connect to the stream if not connected
        liveStreamManager.ConnectLiveStream(streamSetting.StreamKey);

        // Notify clients that the stream is reconnected
        await liveStreamHub.NotifyStreamReconnected(streamSetting.StreamKey);

        // Update stream status to live if not live
        if (streamSetting.Status == LiveStreamStatus.Live)
            return Unit.Value;

        // Update stream status to live
        await liveStreamRepo.UpdateAsync(streamSetting.PublicId, async schedule =>
        {
            schedule.StartStream();
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return Unit.Value;
    }

    private async Task InitStreamInfo(LivestreamSchedule streamSetting, string clientId, CancellationToken ct)
    {
        var streamInfo = new LiveStreamStatusDto
        {
            LocalId = streamSetting.Id,
            Id = streamSetting.PublicId,
            Title = streamSetting.Title,
            Description = streamSetting.Description ?? string.Empty,
            ThumbnailId = streamSetting.ThumbnailId,
            StreamKey = streamSetting.StreamKey,
            LiveAt = streamSetting.StartAt ?? DateTime.UtcNow,
            OfflineAt = streamSetting.EndAt,
            StartTime = streamSetting.StartTime,
            EndTime = streamSetting.EndTime,
            ClientId = clientId,
            AssignedTo = streamSetting.AssignedTo?.Adapt<UserSummaryInfo>(),
            Categories = [.. streamSetting.CategoryMappings.Select(cm => cm.Adapt<LiveStreamCategorySummaryDto>())]
        };
        if (streamSetting.AssignedTo != null && streamSetting.AssignedTo.Avatar != null)
        {
            var avatar = await fileManagerCache.GetFileInfo(streamSetting.AssignedTo.Avatar, ct);
            streamInfo.AssignedTo!.Avatar = avatar?.Url;
        }

        liveStreamManager.InitLiveStream(streamInfo);
    }
}
