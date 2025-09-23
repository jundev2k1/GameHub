using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.EndStream;

public sealed class EndStreamHandler(
    IUserAccessor userAccessor,
    ILiveStreamRepo liveStreamRepo,
    IUnitOfWork unitOfWork,
    ILiveStreamManagerCacheService liveStreamManager,
    ILiveStreamHubService liveStreamHub) : ICommandHandler<EndStreamCommand>
{
    public async Task<Unit> Handle(EndStreamCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var streamSetting = await liveStreamRepo.GetByStreamKeyAsync(request.StreamKey, ct);
        if (streamSetting.AssignedId != userId)
            throw new ForbiddenException("You are not the host of this live stream.");

        await liveStreamRepo.UpdateAsync(request.StreamKey, async schedule =>
        {
            schedule.EndStream();
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);
        liveStreamManager.RemoveLiveStream(request.StreamKey);

        await liveStreamHub.NotifyEndStream(request.StreamKey);
        return Unit.Value;
    }
}
