using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.S2s.DTOs;
using Mapster;

namespace game_x.persistence.Repo;

public sealed class S2sClientSettingRepo(GameXContext dbContext) : IS2sClientSettingRepo, IRepository
{
    public async Task<S2SClientSetting[]> GetAllByClientIdAsync(string clientId, CancellationToken ct = default)
    {
        return await dbContext.S2sClientSettings
            .AsNoTracking()
            .Where(scs => scs.ClientId == clientId)
            .ToArrayAsync(ct);
    }

    public async Task<S2sClientSettingDetailDto> GetDetailAsync(string appCode, CancellationToken ct = default)
    {
        var data = await dbContext.S2sClientSettings
            .AsNoTracking()
            .Include(scs => scs.Client)
            .Include(scs => scs.Credentials)
                .ThenInclude(sc => sc.Materials)
            .FirstOrDefaultAsync(scs => scs.AppCode == appCode, ct)
            ?? throw new NotFoundException(nameof(appCode), appCode);

        return data.Adapt<S2sClientSettingDetailDto>();
    }

    public async Task<S2SClientSetting> GetByAppCodeAsync(string appCode, CancellationToken ct = default)
    {
        return await dbContext.S2sClientSettings
            .AsNoTracking()
            .Include(scs => scs.Client)
            .FirstOrDefaultAsync(scs => scs.AppCode == appCode, ct)
            ?? throw new NotFoundException(nameof(appCode), appCode);
    }

    public async Task CreateAsync(S2SClientSetting entity, CancellationToken ct = default)
    {
        await dbContext.S2sClientSettings.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(string appCode, Action<S2SClientSetting> updateAction, CancellationToken ct = default)
    {
        var target = await dbContext.S2sClientSettings
            .FirstOrDefaultAsync(scs => scs.AppCode == appCode, ct)
            ?? throw new NotFoundException(nameof(appCode), appCode);

        updateAction?.Invoke(target);
    }

    public async Task DeleteAsync(string appCode, CancellationToken ct = default)
    {
        var target = await dbContext.S2sClientSettings
            .FirstOrDefaultAsync(scs => scs.AppCode == appCode, ct)
            ?? throw new NotFoundException(nameof(appCode), appCode);

        dbContext.S2sClientSettings.Remove(target);
    }
}
