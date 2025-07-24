namespace game_x.application.Contract.Persistence.Repo;

public interface IStaffExtensionRepo
{
    Task AddAsync(StaffExtension staffExtension, CancellationToken ct = default);
}