namespace game_x.application.Contract.Persistence.Repo;

public interface IStaffUserRepo
{
    Task AddTrackingLogAsync(StaffUser staffUser, CancellationToken ct = default);
}