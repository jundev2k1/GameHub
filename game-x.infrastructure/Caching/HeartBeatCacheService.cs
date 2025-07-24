using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.HeartBeat.Dtos;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace game_x.infrastructure.Caching;

public sealed class HeartBeatCacheService(IMemoryCache cache)
    : CacheService(cache), IHeartBeatCacheService
{
    private const string CacheKey = "heartbeat:list:today";

    public string GetCounterHeartBeatKey(Guid counterId, string staffId, Guid sessionKey) =>
        $"heartbeat:counter:{counterId}:staff:{staffId}:session:{sessionKey}";

    public (string CounterId, string StaffId, string SessionId)? ParseCounterHeartbeat(string heartbeatId)
    {
        Regex regexPattern = new(
            @"^heartbeat:counter:(?<counterId>[^:]+):staff:(?<staffId>[^:]+):session:(?<sessionKey>.+)$",
            RegexOptions.Compiled);
        if (string.IsNullOrWhiteSpace(heartbeatId)) return null;

        var match = regexPattern.Match(heartbeatId);
        if (!match.Success) return null;

        return (
            CounterId: match.Groups["counterId"].Value,
            StaffId: match.Groups["staffId"].Value,
            SessionId: match.Groups["sessionKey"].Value);
    }

    public List<HeartBeatDto> GetHeartBeatList()
    {
        var result = Get<List<HeartBeatDto>>(CacheKey) ?? [];
        return result;
    }

    public void InsertOrUpdate(string id, Func<HeartBeatDto> getInitCallback)
    {
        var heartBeatList = GetHeartBeatList();
        var targetHeartBeat = heartBeatList.FirstOrDefault(hb => hb.Id == id);
        if (targetHeartBeat is null)
        {
            var dto = getInitCallback();

            dto.LastSeenTime = DateTime.UtcNow;

            heartBeatList.Add(dto);
        }
        else
        {
            targetHeartBeat.LastSeenTime = DateTime.UtcNow;
        }

        Set(CacheKey, heartBeatList);
    }

    public void UpdateStatus(string id, bool isOnline)
    {
        var heartbeatList = GetHeartBeatList();
        var targetHeartBeat = heartbeatList.FirstOrDefault(hb => hb.Id == id);
        if (targetHeartBeat is null) return;

        targetHeartBeat.IsOnline = isOnline;
        targetHeartBeat.LastSeenTime = DateTime.UtcNow;

        Set(CacheKey, heartbeatList);
    }

    public void RemoveItem(string id)
    {
        var newBeatList = GetHeartBeatList()
            .Where(hb => hb.Id != id)
            .ToList();
        Set(CacheKey, newBeatList);
    }

    public void RemoveRange(string[] ids)
    {
        var newBeatList = GetHeartBeatList()
            .Where(hb => ids.Contains(hb.Id) == false)
            .ToList();
        Set(CacheKey, newBeatList);
    }
}
