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
        if (streamSetting.AssignedId is null)
            throw new ForbiddenException("Stream key is not assigned to any talent.");

        var streamInfo = new LiveStreamStatusDto
        {
            StreamKey = streamSetting.StreamKey,
            OfflineAt = null,
            TalentId = streamSetting.AssignedId,
            TalentName = streamSetting.AssignedBy?.Nickname ?? string.Empty,
        };
        liveStreamManager.ConnectLiveStream(streamInfo);

        await liveStreamRepo.UpdateAsync(streamSetting.PublicId, async schedule =>
        {
            if (schedule.Status == LiveStreamStatus.Live)
                return;

            schedule.StartStream();
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return Unit.Value;
    }
}
