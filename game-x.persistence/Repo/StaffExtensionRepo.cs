using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class StaffExtensionRepo(GameXContext context) : IStaffExtensionRepo
{
    public async Task AddAsync(StaffExtension staffExtension, CancellationToken ct = default)
    {
        await context.StaffExtension.AddAsync(staffExtension, ct);
    }
}
