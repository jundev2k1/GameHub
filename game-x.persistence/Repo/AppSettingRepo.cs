using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class AppSettingRepo(GameXContext dbContext) : IAppSettingRepo, IRepository
{
    public async Task<AppSetting[]> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.AppSettings
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    public AppSetting[] GetAll()
    {
        return [.. dbContext.AppSettings.AsNoTracking()];
    }

    public async Task CreateAsync(AppSetting setting, CancellationToken ct = default)
    {
        await dbContext.AppSettings.AddAsync(setting, ct);
    }

    public async Task UpdateAsync(string key, Action<AppSetting> updateAction, CancellationToken ct = default)
    {
        var targetSetting = await dbContext.AppSettings.FirstOrDefaultAsync(s => s.Key == key, ct)
            ?? throw new NotFoundException(nameof(key), key);

        updateAction?.Invoke(targetSetting);
    }
}