using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class StaffCounterRepo(GameXContext context) : IStaffCounterRepo
{
    public async Task<StaffCounter> GetTrackingLogAsync(string userId, CancellationToken ct = default)
    {
        return await context.StaffCounters
            .Include(sc => sc.Counter)
            .OrderBy(sc => sc.LogoutAt != null)
            .ThenByDescending(sc => sc.LoginAt)
            .FirstOrDefaultAsync(sc => (sc.UserId == userId) && (sc.LogoutAt == null), ct)
            ?? throw new NotFoundException(nameof(userId), userId);
    }

    public async Task<bool> IsExistTrackingLogAsync(string userId, int counterId, CancellationToken ct = default)
    {
        return await context.StaffCounters
            .Include(sc => sc.Counter)
            .OrderByDescending(sc => sc.LoginAt)
            .AnyAsync(sc =>
                sc.UserId == userId
                && sc.Counter.Id == counterId
                && sc.LogoutAt == null, ct);
    }

    public async Task<bool> IsCounterInUseByAnotherStaffAsync(int counterId, string staffId, CancellationToken ct = default)
    {
        return await context.StaffCounters
            .AnyAsync(sc =>
                (sc.LogoutAt == null)
                && (sc.Counter.Id == counterId)
                && (sc.UserId != staffId), ct);
    }

    public async Task<bool> IsStaffLoggedInByAnotherCounterAsync(int counterId, string staffId, CancellationToken ct = default)
    {
        return await context.StaffCounters
            .AnyAsync(sc =>
                (sc.LogoutAt == null)
                && (sc.Counter.Id != counterId)
                && (sc.UserId == staffId), ct);
    }

    public async Task AddTrackingLogAsync(StaffCounter staffCounter, CancellationToken ct = default)
    {
        await context.StaffCounters.AddAsync(staffCounter, ct);
    }

    public async Task TrackingLogoutAsync(string userId, int counterId, CancellationToken ct = default)
    {
        var staffCounters = await context.StaffCounters
            .Include(sc => sc.Counter)
            .OrderByDescending(sc => sc.LoginAt)
            .FirstOrDefaultAsync(sc =>
                sc.UserId == userId
                && sc.Counter.Id == counterId
                && sc.LogoutAt == null, ct)
            ?? throw new NotFoundException(nameof(counterId), counterId);

        staffCounters.UpdateLogout();
    }
    public async Task TrackingLogoutAsync(
        string userId,
        Guid counterId,
        DateTime loginAt,
        DateTime? logoutAt = null,
        CancellationToken ct = default)
    {
        var staffCounter = await context.StaffCounters
            .FirstOrDefaultAsync(sc =>
                sc.UserId == userId
                && sc.Counter.PublicId == counterId
                && sc.LoginAt == loginAt, ct)
            ?? throw new NotFoundException(nameof(counterId), counterId);

        staffCounter.UpdateLogout(logoutAt);
    }
}
