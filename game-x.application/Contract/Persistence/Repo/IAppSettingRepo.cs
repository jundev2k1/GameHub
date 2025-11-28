namespace game_x.application.Contract.Persistence.Repo;

public interface IAppSettingRepo
{
    Task<Dictionary<string, string>> GetAllSettingsAsync(CancellationToken ct = default);

    Task<AppSetting[]> GetAllAsync(CancellationToken ct = default);

    Task CreateAsync(AppSetting setting, CancellationToken ct = default);

    Task UpdateAsync(string key, Action<AppSetting> updateAction, CancellationToken ct = default);
}
