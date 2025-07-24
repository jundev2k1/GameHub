using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.HeartBeat.Dtos;

namespace game_x.application.Features.HeartBeat.Staff.Commands.StaffCounterHeartBeat;

public sealed class StaffCounterHeartBeatHandler(
    IHeartBeatCacheService heartBeatCache,
    IStaffCounterRepo staffCounterRepo,
    IUserAccessor userAccessor) : ICommandHandler<StaffCounterHeartBeatCommand>
{
    public async Task<Unit> Handle(StaffCounterHeartBeatCommand request, CancellationToken ct = default)
    {
        var staffId = userAccessor.GetUserId();
        var trackingLog = await staffCounterRepo.GetTrackingLogAsync(staffId, ct);

        var sessionId = heartBeatCache.GetCounterHeartBeatKey(trackingLog.Counter.PublicId, staffId, request.SessionKey);
        var isExist = heartBeatCache.GetHeartBeatList().Any(item => item.Id == sessionId);
        if (!isExist)
            throw new NotFoundException(MessageCode.Staff.SessionKeyNotExist);

        heartBeatCache.InsertOrUpdate(
            sessionId,
            () => new HeartBeatDto
            {
                Id = sessionId,
                LastSeenTime = DateTime.UtcNow,
                LoginTime = trackingLog.LoginAt,
            });
        return Unit.Value;
    }
}
