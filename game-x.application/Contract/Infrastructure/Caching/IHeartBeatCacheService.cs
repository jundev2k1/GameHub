using game_x.application.Features.HeartBeat.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IHeartBeatCacheService
{
    string GetCounterHeartBeatKey(Guid counterId, string staffId, Guid sessionKey);

    (string CounterId, string StaffId, string SessionId)? ParseCounterHeartbeat(string heartbeatId);

    List<HeartBeatDto> GetHeartBeatList();

    void InsertOrUpdate(string id, Func<HeartBeatDto> getInitCallback);

    void UpdateStatus(string id, bool isOnline);

    void RemoveItem(string id);

    void RemoveRange(string[] ids);
}
