using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.PublishStream;

public sealed class PublishStreamHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<PublishStreamCommand>
{
    public async Task<Unit> Handle(PublishStreamCommand request, CancellationToken ct = default)
    {
        var streamSetting = await liveStreamRepo
            .GetByStreamKeyAsync(request.StreamKey, ct);
        if (streamSetting.Token != request.Token)
            throw new ForbiddenException("Token is invalid.");
        if (streamSetting.AssignedId is null)
            throw new ForbiddenException("Stream key is not assigned to any talent.");

        // Initialize stream info in cache if not exists
        if (!liveStreamManager.IsExistLiveStream(streamSetting.StreamKey))
            InitStreamInfo(streamSetting);

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

    private void InitStreamInfo(LivestreamSchedule streamSetting)
    {
        var streamInfo = new LiveStreamStatusDto
        {
            StreamKey = streamSetting.StreamKey,
            OfflineAt = null,
            StartTime = streamSetting.StartTime,
            EndTime = streamSetting.EndTime,
            TalentId = streamSetting.AssignedId!,
            TalentName = streamSetting.AssignedTo?.Nickname ?? string.Empty,
            Categories = [.. streamSetting.CategoryMappings.Select(cm => cm.Adapt<LiveStreamCategorySummaryDto>())]
        };
        liveStreamManager.InitLiveStream(streamInfo);
    }
}
