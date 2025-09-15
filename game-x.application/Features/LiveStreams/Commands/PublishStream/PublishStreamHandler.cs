using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.LiveStreams.Commands.PublishStream;

public sealed class PublishStreamHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IOptions<SrsSettings> settings) : ICommandHandler<PublishStreamCommand>
{
    public async Task<Unit> Handle(PublishStreamCommand request, CancellationToken ct = default)
    {
        if (!settings.Value.StreamServer.EndsWith(request.Server))
            throw new BadRequestException("Server Url invalid.");

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
            schedule.StartStream();
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
