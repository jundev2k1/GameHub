using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class S2sCredentialRepo(GameXContext dbContext) : IS2sCredentialRepo, IRepository
{
    public async Task<S2SCredential[]> GetByKeyIdAsync(string keyId, CancellationToken ct = default)
    {
        return await dbContext.S2sCredentials
            .AsNoTracking()
            .Include(sc => sc.Materials)
            .Where(sc => sc.KeyId == keyId)
            .ToArrayAsync(ct);
    }

    public async Task<S2SCredential[]> GetsBySettingAsync(int settingId, CancellationToken ct = default)
    {
        return await dbContext.S2sCredentials
            .AsNoTracking()
            .Include(sc => sc.Materials)
            .Where(sc => sc.SettingId == settingId)
            .ToArrayAsync(ct);
    }

    public async Task CreateAsync(S2SCredential entity, CancellationToken ct = default)
    {
        await dbContext.S2sCredentials.AddAsync(entity, ct);
    }

    public async Task CreateRangeAsync(IEnumerable<S2SCredential> entities, CancellationToken ct = default)
    {
        await dbContext.S2sCredentials.AddRangeAsync(entities, ct);
    }

    public async Task UpdateAsync(string keyId, Func<S2SCredential, Task> updateAction, CancellationToken ct = default)
    {
        var target = await dbContext.S2sCredentials
            .Include(sc => sc.Materials)
            .FirstOrDefaultAsync(sc => sc.KeyId == keyId, ct)
            ?? throw new NotFoundException(nameof(keyId), keyId);

        await updateAction(target);
    }
}
