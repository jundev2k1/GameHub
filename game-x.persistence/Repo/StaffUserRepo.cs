using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class StaffUserRepo(GameXContext context) : IStaffUserRepo
{
    public async Task AddTrackingLogAsync(StaffUser staffUser, CancellationToken ct = default)
    {
        await context.StaffUsers.AddAsync(staffUser, ct);
    }
}
