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

    public async Task<S2SCredential> GetByKeyIdAsync(string keyId, CredentialDirection direction, CancellationToken ct = default)
    {
        return await dbContext.S2sCredentials
            .AsNoTracking()
            .Include(sc => sc.Materials)
            .Include(sc => sc.ClientSetting)
            .FirstOrDefaultAsync(sc => sc.KeyId == keyId && sc.Direction == direction, ct)
            ?? throw new NotFoundException(nameof(keyId), keyId);
    }

    public async Task<S2SCredential?> GetActiveCredentialAsync(string appCode, CredentialDirection direction, CancellationToken ct = default)
    {
        return await dbContext.S2sCredentials
            .AsNoTracking()
            .OrderByDescending(sc => sc.CreatedAt)
            .FirstOrDefaultAsync(sc => sc.ClientSetting.AppCode == appCode
                && sc.ClientSetting.IsActive
                && sc.Direction == direction
                && sc.Status == CredentialStatus.Active, ct);
    }

    public async Task<S2SCredential[]> GetsBySettingAsync(int settingId, CancellationToken ct = default)
    {
        return await dbContext.S2sCredentials
            .AsNoTracking()
            .Include(sc => sc.Materials)
            .Where(sc => sc.SettingId == settingId)
            .ToArrayAsync(ct);
    }

    public async Task<S2SCredential?> GetsByKeyIdAsync(string keyId, CancellationToken ct = default)
    {
        return await dbContext.S2sCredentials
            .AsNoTracking()
            .Include(sc => sc.Materials)
            .FirstOrDefaultAsync(sc => sc.KeyId == keyId, ct);
    }

    public async Task<bool> CanAddKeyAsync(string appCode, CredentialDirection direction, CancellationToken ct = default)
    {
        return await dbContext.S2sClientSettings
            .AsNoTracking()
            .AnyAsync(scs => scs.AppCode == appCode
                && scs.IsActive
                && !scs.Credentials.Any(c =>
                    c.Direction == direction
                    && c.Status == CredentialStatus.Active), ct);
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
            .Include(sc => sc.ClientSetting)
            .Include(sc => sc.Materials)
            .FirstOrDefaultAsync(sc => sc.KeyId == keyId, ct)
            ?? throw new NotFoundException(nameof(keyId), keyId);

        await updateAction(target);
    }

    public async Task RotateAsync(string keyId, S2SCredential credential, CancellationToken ct = default)
    {
        var target = await dbContext.S2sCredentials
            .FirstOrDefaultAsync(sc => sc.KeyId == keyId && sc.Direction == CredentialDirection.Inbound, ct)
            ?? throw new NotFoundException(nameof(keyId), keyId);

        target.Deactivate();
        await dbContext.S2sCredentials.AddAsync(credential, ct);
    }
}
