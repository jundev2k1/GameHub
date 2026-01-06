using game_x.application.Features.S2s.DTOs;

namespace game_x.application.Contract.Persistence.Repo;

public interface IS2sClientSettingRepo
{
    Task<S2SClientSetting[]> GetAllByClientIdAsync(string clientId, CancellationToken ct = default);

    Task<S2sClientSettingDetailDto> GetDetailAsync(string appCode, CancellationToken ct = default);

    Task CreateAsync(S2SClientSetting entity, CancellationToken ct = default);

    Task UpdateAsync(string appCode, Action<S2SClientSetting> updateAction, CancellationToken ct = default);

    Task DeleteAsync(string appCode, CancellationToken ct = default);
}
