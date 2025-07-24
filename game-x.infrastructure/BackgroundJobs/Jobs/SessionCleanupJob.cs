using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.HeartBeat.Dtos;
using game_x.domain.Entities;
using game_x.share.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class SessionCleanupJob(
    IHeartBeatCacheService heartBeatCache,
    IStaffCounterRepo staffCounterRepo,
    GameXContext dbContext,
    IUnitOfWork unitOfWork,
    IOptions<RecurringJobSettings> jobOptions,
    ILogger<SessionCleanupJob> logger) : IRecurringJob
{
    public string JobId => "session-cleanup";
    public string CronExpression => jobOptions.Value.SessionCleanUpJob;
    public bool IsInit => true;

    /// <summary>Kill sessions that have been suspended for too long (Minutes)</summary>
    private const int CleanupAfterMinutes = 30;
    /// <summary>Maximum time a session can survive (Only applies to zombie sessions) (Hours)</summary>
    private const int SessionTimeout = 8;
    /// <summary>Maximum number of records allowed to be processed per transaction</summary>
    private const int LimitRangeCount = 1000;

    /// <summary>StaffCounter cache heartbeat prefix key</summary>
    private readonly string _prefixHeartbeatCounter = "heartbeat:counter:";

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        // Clean up all suspended sessions
        await KillZombieSessions(ct);

        // Get expired heart beats
        var expirationThreshold = DateTime.UtcNow.AddMinutes(-CleanupAfterMinutes);
        var expiredHeartBeats = heartBeatCache.GetHeartBeatList()
            .Where(hb => !hb.IsOnline && hb.LastSeenTime < expirationThreshold)
            .ToArray();

        // Write result log
        if (expiredHeartBeats.Length > 0)
        {
            logger.LogInformation("Running SessionCleanupJob at {Time}", DateTime.UtcNow);
            logger.LogInformation(
                "Found {Count} expired sessions before {Threshold}",
                expiredHeartBeats.Length,
                expirationThreshold);
        }

        // Stop if there is no expired heart beats
        if (expiredHeartBeats.Length == 0)
            return;

        var expiredIds = expiredHeartBeats
            .Select(hb => hb.Id)
            .ToArray();
        heartBeatCache.RemoveRange(expiredIds);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Handle logout for staff counter session
            await AutoLogoutStaffCounter(expiredHeartBeats, ct);
        }, ct);
    }

    private async Task AutoLogoutStaffCounter(HeartBeatDto[] heartBeats, CancellationToken ct)
    {
        var getStaffCounterInfo = (HeartBeatDto hb) =>
        {
            var heartBeatInfo = heartBeatCache.ParseCounterHeartbeat(hb.Id);
            if (heartBeatInfo is null) return default;

            return (
                staffId: heartBeatInfo.Value.StaffId,
                CounterId: Guid.Parse(heartBeatInfo.Value.CounterId),
                LoginAt: hb.LoginTime,
                hb.LastSeenTime);
        };

        var expirationThreshold = DateTime.UtcNow.AddMinutes(-CleanupAfterMinutes);
        var currentHeartbeats = heartBeatCache.GetHeartBeatList()
            .Where(hb => hb.IsOnline && hb.Id.StartsWith(_prefixHeartbeatCounter))
            .Select(getStaffCounterInfo)
            .ToArray();
        var targetHeartBeats = heartBeats
            .Where(hb => hb.Id.StartsWith(_prefixHeartbeatCounter))
            .Select(getStaffCounterInfo)
            .Where(info => !currentHeartbeats.Any(item => item.staffId == info.staffId && item.CounterId == info.CounterId))
            .ToList();
        foreach (var (staffId, CounterId, LoginAt, LastSeenTime) in targetHeartBeats)
        {
            await staffCounterRepo.TrackingLogoutAsync(
                userId: staffId,
                counterId: CounterId,
                loginAt: LoginAt,
                logoutAt: LastSeenTime, ct);
        }
    }

    private async Task KillZombieSessions(CancellationToken ct)
    {
        // Get all the living heartbeats
        var loginTimeHeartbeats = heartBeatCache.GetHeartBeatList()
            .Where(hb => hb.Id.StartsWith(_prefixHeartbeatCounter))
            .Select(hb => hb.LoginTime)
            .ToArray();
        var zombieSessionCount = await dbContext.StaffCounters
            .Where(sc => (sc.LogoutAt == null)
                && (loginTimeHeartbeats.Contains(sc.LoginAt) == false))
            .CountAsync(ct);
        if (zombieSessionCount == 0) return;

        logger.LogInformation("- Start searching and destroying zombie sessions.");

        int count = 0;
        bool isContinue = true;
        StaffCounter? unProcessStaffCounter = null;
        var sessionNeedUpdates = new List<StaffCounter>();
        // Loop through the zombie sessions in batches
        while (isContinue)
        {
            var zombieSessions = await dbContext.StaffCounters
                .AsNoTracking()
                .OrderBy(sc => sc.LoginAt)
                .Skip(count)
                .Take(LimitRangeCount)
                .ToArrayAsync(ct);

            // If there are no zombie sessions left, break the loop or handle for last session
            if (zombieSessions.Any() == false)
            {
                // No more zombie sessions to process
                isContinue = false;
                if (unProcessStaffCounter is null) break;

                // If there is an unprocessed staff counter, handle it as the last item
                var lastCounterNeedUpdates = CompareAndSetLogoutTime([unProcessStaffCounter], null, true).ToList();
                if (lastCounterNeedUpdates.Any()) sessionNeedUpdates.AddRange(lastCounterNeedUpdates);
                break;
            }

            // Handle compare and set logout time for zombie sessions
            var counterNeedUpdates = CompareAndSetLogoutTime(zombieSessions, unProcessStaffCounter).ToList();
            if (counterNeedUpdates.Any())
                sessionNeedUpdates.AddRange(counterNeedUpdates);

            unProcessStaffCounter = zombieSessions.LastOrDefault(sc => sc.LogoutAt == null);
            count = count + LimitRangeCount;
        }

        // Update the logout time for all zombie sessions
        foreach (var zombieSessionGroup in sessionNeedUpdates.Chunk(LimitRangeCount))
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                dbContext.StaffCounters.UpdateRange(zombieSessionGroup);
                await Task.CompletedTask;
            }, ct);
            logger.LogInformation("+ Killed {count} zombie sessions.", zombieSessionGroup.Length);
        }

        logger.LogInformation("- End searching and destroying zombie sessions.");
    }

    private static IEnumerable<StaffCounter> CompareAndSetLogoutTime(
        StaffCounter[] staffCounters,
        StaffCounter? unProcessStaffCounter,
        bool isLastItem = false)
    {
        for (var index = 0; index < staffCounters.Length; index++)
        {
            // If there is outstanding session in the previous iteration
            // Check [SessionTimeout] for greater than current session? Choose shortest time.
            if ((index == 0) && (unProcessStaffCounter != null))
            {
                var logoutTimeout = staffCounters[index].LoginAt - unProcessStaffCounter.LoginAt;
                var logoutTime = logoutTimeout.Hours < SessionTimeout
                    ? staffCounters[index].LoginAt
                    : unProcessStaffCounter.LoginAt.AddHours(SessionTimeout);
                unProcessStaffCounter.UpdateLogout(logoutTime);
                yield return unProcessStaffCounter;
            }

            // If there is no outstanding session in the previous iteration
            // Check [SessionTimeout] for greater than next session? Choose shortest time.
            if ((index < staffCounters.Length - 1) && (staffCounters[index].LogoutAt is null))
            {
                var logoutTimeout = staffCounters[index + 1].LoginAt - staffCounters[index].LoginAt;
                var logoutTime = logoutTimeout.Hours < SessionTimeout
                    ? staffCounters[index + 1].LoginAt
                    : staffCounters[index].LoginAt.AddHours(SessionTimeout);
                staffCounters[index].UpdateLogout(logoutTime);
                yield return staffCounters[index];
            }

            // If the session is the last one and not logged out
            // Check [SessionTimeout] for greater than current time? Choose shortest time.
            if (isLastItem && (index == staffCounters.Length - 1) && (staffCounters[index].LogoutAt is null))
            {
                var logoutTime = DateTime.UtcNow.Hour > SessionTimeout
                    ? staffCounters[index].LoginAt.AddHours(SessionTimeout)
                    : DateTime.UtcNow;
                staffCounters[index].UpdateLogout(logoutTime);
                yield return staffCounters[index];
            }
        }
    }
}
