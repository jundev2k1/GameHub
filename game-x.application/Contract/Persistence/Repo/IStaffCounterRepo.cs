namespace game_x.application.Contract.Persistence.Repo;

public interface IStaffCounterRepo
{
    Task<StaffCounter> GetTrackingLogAsync(string userId, CancellationToken ct = default);

    Task<bool> IsExistTrackingLogAsync(string userId, int counterId, CancellationToken ct = default);

    Task<bool> IsCounterInUseByAnotherStaffAsync(int counterId, string staffId, CancellationToken ct = default);

    Task<bool> IsStaffLoggedInByAnotherCounterAsync(int counterId, string staffId, CancellationToken ct = default);

    Task AddTrackingLogAsync(StaffCounter staffCounter, CancellationToken ct = default);

    Task TrackingLogoutAsync(string userId, int counterId, CancellationToken ct = default);
    Task TrackingLogoutAsync(string userId, Guid counterId, DateTime loginAt, DateTime? logoutAt = null, CancellationToken ct = default);
}