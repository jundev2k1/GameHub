using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Dtos;
using System.Threading.Tasks;

namespace game_x.application.Features.LiveStreams.Commands.PublishStream;

public sealed class PublishStreamHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<PublishStreamCommand>
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
            await InitStreamInfo(streamSetting, ct);

        // Connect to the stream if not connected
        liveStreamManager.ConnectLiveStream(streamSetting.StreamKey);

        // Update stream status to live if not live
        if (streamSetting.Status == LiveStreamStatus.Live)
            return Unit.Value;

        await liveStreamRepo.UpdateAsync(streamSetting.PublicId, async schedule =>
        {
            schedule.StartStream();
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return Unit.Value;
    }

    private async Task InitStreamInfo(LivestreamSchedule streamSetting, CancellationToken ct)
    {
        var streamInfo = new LiveStreamStatusDto
        {
            Id = streamSetting.PublicId,
            StreamKey = streamSetting.StreamKey,
            OfflineAt = null,
            StartTime = streamSetting.StartTime,
            EndTime = streamSetting.EndTime,
            AssignedTo = streamSetting.AssignedTo?.Adapt<UserSummaryInfo>(),
            Categories = [.. streamSetting.CategoryMappings.Select(cm => cm.Adapt<LiveStreamCategorySummaryDto>())]
        };
        if (streamSetting.AssignedTo != null && streamSetting.AssignedTo.Avatar != null)
        {
            var avatar = await fileManagerCache.GetImageUrl(streamSetting.AssignedTo.Avatar, ct);
            streamInfo.AssignedTo!.Avatar = avatar?.Url;
        }

        liveStreamManager.InitLiveStream(streamInfo);
    }
}
