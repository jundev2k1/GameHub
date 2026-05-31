namespace game_x.application.Contract.Persistence.Repo;

public interface IAppSettingRepo
{
    Task<AppSetting[]> GetAllAsync(CancellationToken ct = default);

    AppSetting[] GetAll();

    Task CreateAsync(AppSetting setting, CancellationToken ct = default);

    Task UpdateAsync(string key, Action<AppSetting> updateAction, CancellationToken ct = default);
}