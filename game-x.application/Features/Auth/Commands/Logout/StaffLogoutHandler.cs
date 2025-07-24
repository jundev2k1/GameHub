using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Commands.Logout;

public sealed class StaffLogoutHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IStaffCounterRepo staffCounterRepo,
    IHeartBeatCacheService heartBeatCache) : ICommandHandler<StaffLogoutCommand>
{
    public async Task<Unit> Handle(StaffLogoutCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var staffCounter = await staffCounterRepo.GetTrackingLogAsync(userId, ct);

        // Check if the user is logged in at the counter
        RemoveSessionHeartBeat(userId, staffCounter.Counter.PublicId, request.SessionKey);

        await staffCounterRepo.TrackingLogoutAsync(userId, staffCounter.Counter.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }

    private void RemoveSessionHeartBeat(string staffId, Guid counterId, Guid sessionKey)
    {
        var sessionId = heartBeatCache.GetCounterHeartBeatKey(counterId, staffId, sessionKey);
        var isExist = heartBeatCache.GetHeartBeatList().Any(item => item.Id == sessionId);
        if (!isExist) throw new NotFoundException(MessageCode.Staff.SessionKeyNotExist);

        heartBeatCache.RemoveItem(sessionId);
    }
}
